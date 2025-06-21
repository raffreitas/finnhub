using System.Text.Json.Serialization;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Models;

public record Ticker24HrModel(
    [property: JsonPropertyName("e")] string EventType,
    [property: JsonPropertyName("E")] long EventTime,
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("p")] string PriceChange,
    [property: JsonPropertyName("P")] string PriceChangePercent,
    [property: JsonPropertyName("w")] string WeightedAveragePrice,
    [property: JsonPropertyName("x")] string FirstTradeBeforeWindowPrice,
    [property: JsonPropertyName("c")] string LastPrice,
    [property: JsonPropertyName("Q")] string LastQuantity,
    [property: JsonPropertyName("b")] string BestBidPrice,
    [property: JsonPropertyName("B")] string BestBidQuantity,
    [property: JsonPropertyName("a")] string BestAskPrice,
    [property: JsonPropertyName("A")] string BestAskQuantity,
    [property: JsonPropertyName("o")] string OpenPrice,
    [property: JsonPropertyName("h")] string HighPrice,
    [property: JsonPropertyName("l")] string LowPrice,
    [property: JsonPropertyName("v")] string TotalTradedBaseAssetVolume,
    [property: JsonPropertyName("q")] string TotalTradedQuoteAssetVolume,
    [property: JsonPropertyName("O")] long StatisticsOpenTime,
    [property: JsonPropertyName("C")] long StatisticsCloseTime,
    [property: JsonPropertyName("F")] long FirstTradeId,
    [property: JsonPropertyName("L")] long LastTradeId,
    [property: JsonPropertyName("n")] long TotalNumberOfTrades
);
