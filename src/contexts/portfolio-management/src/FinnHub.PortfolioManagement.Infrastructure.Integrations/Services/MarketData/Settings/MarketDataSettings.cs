using System.ComponentModel.DataAnnotations;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Services.MarketData.Settings;

internal sealed record MarketDataSettings
{
    public const string SectionName = nameof(MarketDataSettings);

    [Required, Url]
    public required string BaseUrl { get; init; }
}
