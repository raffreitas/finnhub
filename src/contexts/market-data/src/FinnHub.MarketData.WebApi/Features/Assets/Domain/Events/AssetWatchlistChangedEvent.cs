using FinnHub.MarketData.WebApi.Features.Assets.Domain.Enums;

namespace FinnHub.MarketData.WebApi.Features.Assets.Domain.Events;

public sealed record AssetWatchlistChangedEvent(
    Guid Id,
    string Exchange,
    AssetChangedType Action,
    string Symbol,
    AssetType AssetType,
    DateTimeOffset OccurredOn
);
