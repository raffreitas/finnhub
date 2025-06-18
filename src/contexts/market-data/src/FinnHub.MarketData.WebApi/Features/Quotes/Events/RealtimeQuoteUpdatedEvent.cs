using FinnHub.MarketData.WebApi.Shared.Abstractions;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Events;

public sealed record RealtimeQuoteUpdatedEvent(
    string AssetSymbol,
    decimal Price,
    DateTime Timestamp,
    long? Volume
) : IDomainEvent;
