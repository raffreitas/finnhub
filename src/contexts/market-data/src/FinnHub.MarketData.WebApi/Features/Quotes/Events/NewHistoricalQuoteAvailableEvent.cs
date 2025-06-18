using FinnHub.MarketData.WebApi.Shared.Abstractions;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Events;

public record NewHistoricalQuoteAvailableEvent(
    string AssetSymbol,
    DateOnly Date,
    decimal ClosePrice,
    string Interval
) : IDomainEvent;
