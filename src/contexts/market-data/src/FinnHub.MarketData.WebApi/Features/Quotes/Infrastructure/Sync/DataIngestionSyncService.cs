using System.Diagnostics;

using FinnHub.MarketData.WebApi.Features.Quotes.Commands.IngestMarketData;
using FinnHub.MarketData.WebApi.Features.Quotes.Commands.SaveMarketData;
using FinnHub.MarketData.WebApi.Features.Quotes.Domain.Events;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Sync;

internal sealed class DataIngestionSyncService(
    ILogger<DataIngestionSyncService> logger,
    IMessageBus messageBus,
    IServiceProvider serviceProvider
) : BackgroundService
{
    private readonly ActivitySource _activitySource = new("FinnHub.MarketData.Sync");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<MarketDataIngestedEvent>(HandleDataReceived, stoppingToken);
    }

    private async Task HandleDataReceived(MarketDataIngestedEvent @event, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("handle_market_data");
        activity?.SetTag("market_data.symbol", @event.Symbol);
        activity?.SetTag("market_data.price", @event.LastPrice);
        activity?.SetTag("market_data.source", @event.Source);

        try
        {
            using var scope = serviceProvider.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<SaveMarketDataHandler>();

            var command = new SaveMarketDataCommand(
                @event.Symbol,
                @event.LastPrice,
                @event.OpenPrice,
                @event.HighPrice,
                @event.LowPrice,
                @event.Volume,
                @event.PriceChange,
                @event.PriceChangePercent,
                @event.Timestamp,
                @event.Source
            );

            logger.LogInformation("ReceivedEvent {Timestamp} for {Symbol}", command.Timestamp, command.Symbol);

            await handler.Handle(command, cancellationToken);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling market data for {Symbol}", @event.Symbol);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }
}
