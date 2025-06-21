using System.ComponentModel.DataAnnotations;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Infrastructure.Binance.Settings;

internal sealed record BinanceSettings
{
    public const string SectionName = nameof(BinanceSettings);

    [Required, MinLength(1)]
    public required string Uri { get; set; }
}
