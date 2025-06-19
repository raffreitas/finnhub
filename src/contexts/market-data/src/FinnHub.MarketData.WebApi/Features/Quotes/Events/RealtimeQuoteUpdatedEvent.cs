using FinnHub.MarketData.WebApi.Shared.Domain.Abstractions;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Events;

public sealed record RealtimeQuoteUpdatedEvent(
    string AssetSymbol,
    decimal Price,
    DateTime Timestamp,
    long? Volume
) : IDomainEvent;
