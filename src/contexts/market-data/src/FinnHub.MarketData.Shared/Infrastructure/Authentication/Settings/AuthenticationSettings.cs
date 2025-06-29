using System.ComponentModel.DataAnnotations;

namespace FinnHub.MarketData.Shared.Infrastructure.Authentication.Settings;

public sealed record AuthenticationSettings
{
    public const string SectionName = nameof(AuthenticationSettings);

    [Required, MinLength(32)]
    public required string JwtSecret { get; init; }

    [Required]
    public TimeSpan JwtExpiration { get; init; }
}
