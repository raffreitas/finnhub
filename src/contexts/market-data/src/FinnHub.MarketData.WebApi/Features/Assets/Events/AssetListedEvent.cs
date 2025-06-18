using FinnHub.MarketData.WebApi.Shared.Abstractions;

namespace FinnHub.MarketData.WebApi.Features.Assets.Events;

public sealed record AssetListedEvent(
    string Symbol,
    string Name,
    string Type,
    string Exchange
) : IDomainEvent;
