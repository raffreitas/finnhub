using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using FinnHub.MarketData.WebApi.Features.Assets.Domain.Enums;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Events;
using FinnHub.MarketData.WebApi.Features.Assets.Domain.Repositories;
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
    IMessageBus messageBus,
    IServiceProvider serviceProvider
) : BackgroundService
{
    private readonly BinanceSettings binanceSettings = options.Value;
    private readonly ActivitySource _activitySource = new("FinnHub.MarketData.Binance");
    private readonly ConcurrentBag<string> _subscribedSymbols = [];
    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);
    private ClientWebSocket _webSocket = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeSubscriptionsAsync(stoppingToken);

        await messageBus.SubscribeAsync<AssetWatchlistChangedEvent>(HandleAssetWatchlistChanged, stoppingToken);

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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisposeWebSocketAsync();
        _activitySource.Dispose();
        _connectionSemaphore.Dispose();
        await base.StopAsync(cancellationToken);
    }

    private async Task InitializeSubscriptionsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var activeAssets = await assetRepository.GetAllActiveAsync(cancellationToken);

        foreach (var asset in activeAssets)
        {
            _subscribedSymbols.Add(asset.Symbol.ToLowerInvariant());
        }
    }

    private async Task HandleAssetWatchlistChanged(AssetWatchlistChangedEvent @event, CancellationToken cancellationToken)
    {
        if (
            !@event.Exchange.Equals(Exchange.Binance.ToString(), StringComparison.OrdinalIgnoreCase) ||
            !@event.AssetType.Equals(AssetType.Crypto.ToString(), StringComparison.OrdinalIgnoreCase)
        )
        {
            return;
        }

        logger.LogInformation("Handling asset watchlist changed event for {Symbol}. Action: {Action}", @event.Symbol, @event.Action);

        using var activity = _activitySource.StartActivity("handle_binance_watchlist_changed");

        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (@event.Action == AssetChangedType.Added.ToString())
            {
                _subscribedSymbols.Add(@event.Symbol);
            }
            else if (@event.Action == AssetChangedType.Removed.ToString())
            {
                string[] symbolsToRemove = [@event.Symbol];
                var newSymbols = new ConcurrentBag<string>(_subscribedSymbols.Except(symbolsToRemove));
                while (!_subscribedSymbols.IsEmpty) _subscribedSymbols.TryTake(out _);
                foreach (var symbol in newSymbols) _subscribedSymbols.Add(symbol);
            }

            await ForceReconnectAsync(cancellationToken);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    private async Task ConnectWebSocketClientAsync(CancellationToken cancellationToken)
    {
        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_subscribedSymbols.IsEmpty)
            {
                logger.LogWarning("No symbols to subscribe to on Binance WebSocket.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                return;
            }

            if (_webSocket?.State != WebSocketState.None && _webSocket?.State != WebSocketState.Closed)
            {
                await DisposeWebSocketAsync();
            }

            _webSocket ??= new ClientWebSocket();

            if (_webSocket.State != WebSocketState.None)
            {
                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();
            }

            var streams = string.Join("/", _subscribedSymbols.Select(s => $"{s.ToLower()}@ticker"));
            Uri uri = new($"{binanceSettings.Uri}stream?streams={streams}");

            logger.LogInformation("Connecting to Binance WebSocket with streams: {Streams}", streams);

            await _webSocket.ConnectAsync(uri, cancellationToken);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    private async Task ForceReconnectAsync(CancellationToken cancellationToken)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting with updated symbol list", cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while closing WebSocket connection during force reconnect. It might already be closed or disposed.");
            }
        }

        await DisposeWebSocketAsync();
    }

    private async Task DisposeWebSocketAsync()
    {
        if (_webSocket != null)
        {
            try
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while closing WebSocket connection. It might already be closed or disposed.");
            }
            finally
            {
                _webSocket?.Dispose();
            }
        }
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
            var marketData = JsonSerializer.Deserialize<CombinedStreamModel<Ticker24HrModel>>(message);
            if (marketData is null) return;

            activity?.SetTag("binance.symbol", marketData.Data.Symbol);
            activity?.SetTag("binance.price", marketData.Data.LastPrice);
            activity?.SetTag("binance.event_time", marketData.Data.EventTime);

            var quoteEvent = new MarketDataIngestedEvent(
                Id: Guid.NewGuid(),
                Symbol: marketData.Data.Symbol,
                LastPrice: decimal.Parse(marketData.Data.LastPrice),
                OpenPrice: decimal.Parse(marketData.Data.OpenPrice),
                HighPrice: decimal.Parse(marketData.Data.HighPrice),
                LowPrice: decimal.Parse(marketData.Data.LowPrice),
                Volume: decimal.Parse(marketData.Data.TotalTradedBaseAssetVolume),
                PriceChange: decimal.Parse(marketData.Data.PriceChange),
                PriceChangePercent: decimal.Parse(marketData.Data.PriceChangePercent),
                Timestamp: DateTimeOffset.FromUnixTimeMilliseconds(marketData.Data.EventTime),
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
