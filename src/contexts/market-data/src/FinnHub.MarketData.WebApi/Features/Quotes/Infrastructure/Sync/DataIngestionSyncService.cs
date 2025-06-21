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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.SubscribeAsync<MarketDataIngestedEvent>(HandleDataReceived, stoppingToken);
    }

    private async Task HandleDataReceived(MarketDataIngestedEvent @event, CancellationToken cancellationToken)
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

        logger.LogInformation("ReceivedEvent {Timestamp}", command.Timestamp);

        await handler.Handle(command, cancellationToken);
    }
}
