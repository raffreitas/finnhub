using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Enums;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Events;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Models;
using FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Settings;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services;

using Microsoft.Extensions.Options;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance;

internal sealed class BinanceDataIngestionService(
    IOptions<BinanceSettings> options,
    ILogger<BinanceDataIngestionService> logger,
    IMessageBus messageBus
) : BackgroundService
{
    private readonly BinanceSettings binanceSettings = options.Value;
    private readonly ClientWebSocket _webSocket = new();
    private readonly ActivitySource _activitySource = new("FinnHub.MarketData.Binance");
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectWebSocketClientAsync(stoppingToken);
                await ListenToWebSocketAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WebSocket connection error. Reconnecting in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ConnectWebSocketClientAsync(CancellationToken cancellationToken)
    {
        Uri uri = new(binanceSettings.Uri + "btcusdt@ticker");
        await _webSocket.ConnectAsync(uri, cancellationToken);
    }

    private async Task ListenToWebSocketAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];

        while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await _webSocket.ReceiveAsync(buffer, cancellationToken);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await ProcessWebSocketMessage(json);
            }
        }
    }

    private async Task ProcessWebSocketMessage(string message)
    {
        using var activity = _activitySource.StartActivity("process_binance_message");

        try
        {
            var marketData = JsonSerializer.Deserialize<Ticker24HrModel>(message);
            if (marketData is null) return;

            activity?.SetTag("binance.symbol", marketData.Symbol);
            activity?.SetTag("binance.price", marketData.LastPrice);
            activity?.SetTag("binance.event_time", marketData.EventTime);

            var quoteEvent = new MarketDataIngestedEvent(
                Id: Guid.NewGuid(),
                Symbol: marketData.Symbol,
                LastPrice: decimal.Parse(marketData.LastPrice),
                OpenPrice: decimal.Parse(marketData.OpenPrice),
                HighPrice: decimal.Parse(marketData.HighPrice),
                LowPrice: decimal.Parse(marketData.LowPrice),
                Volume: decimal.Parse(marketData.TotalTradedBaseAssetVolume),
                PriceChange: decimal.Parse(marketData.PriceChange),
                PriceChangePercent: decimal.Parse(marketData.PriceChangePercent),
                Timestamp: DateTimeOffset.FromUnixTimeMilliseconds(marketData.EventTime),
                Source: Exchange.Binance.ToString()
            );

            await messageBus.PublishAsync(quoteEvent);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing WebSocket message: {Message}", message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
        }
    }
}
