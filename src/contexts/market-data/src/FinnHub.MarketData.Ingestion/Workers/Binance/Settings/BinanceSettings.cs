using System.ComponentModel.DataAnnotations;

namespace FinnHub.MarketData.Ingestion.Workers.Binance.Settings;

internal sealed record BinanceSettings
{
    public const string SectionName = nameof(BinanceSettings);

    [Required, MinLength(1)]
    public required string Uri { get; set; }
}
