using System.ComponentModel.DataAnnotations;

namespace FinnHub.PortfolioManagement.Infrastructure.Caching.Settings;
internal sealed record CacheSettings
{
    public const string SectionName = "CacheSettings";

    [Required, Url]
    public required string ConnectionString { get; init; }

    [Required, MinLength(1)]
    public required string CacheName { get; init; }

    public int DefaultExpirationInMinutes { get; init; } = 60;
}
