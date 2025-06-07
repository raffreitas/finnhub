using System.ComponentModel.DataAnnotations;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Settings;
internal sealed record DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";

    [Required, Url]
    public required string ConnectionString { get; init; }
    public int CommandTimeoutInSeconds { get; init; } = 180;
}
