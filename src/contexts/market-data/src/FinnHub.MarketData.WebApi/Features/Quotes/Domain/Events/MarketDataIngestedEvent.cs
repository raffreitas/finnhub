namespace FinnHub.MarketData.WebApi.Features.Quotes.Domain.Events;

public sealed record MarketDataIngestedEvent(
    Guid Id,
    string Symbol,
    decimal LastPrice,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal Volume,
    decimal PriceChange,
    decimal PriceChangePercent,
    DateTimeOffset Timestamp,
    string Source
);
