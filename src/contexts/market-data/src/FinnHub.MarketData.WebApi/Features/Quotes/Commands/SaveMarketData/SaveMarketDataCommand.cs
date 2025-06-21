namespace FinnHub.MarketData.WebApi.Features.Quotes.Commands.IngestMarketData;

public sealed record SaveMarketDataCommand(
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
