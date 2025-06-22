namespace FinnHub.MarketData.WebApi.Features.Assets.Domain.Events;

public sealed record AssetWatchlistChangedEvent(
    Guid Id,
    string Exchange,
    string Action,
    string Symbol,
    string AssetType,
    DateTimeOffset OccurredOn
);
