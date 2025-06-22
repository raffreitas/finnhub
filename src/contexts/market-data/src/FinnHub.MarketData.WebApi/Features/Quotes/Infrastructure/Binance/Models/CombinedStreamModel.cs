using System.Text.Json.Serialization;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Models;

internal sealed record CombinedStreamModel<T>
{
    [JsonPropertyName("stream")]
    public required string Stream { get; init; } = string.Empty;

    [JsonPropertyName("data")]
    public required T Data { get; init; } = default!;
}
