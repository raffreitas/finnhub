using System.ComponentModel.DataAnnotations;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Settings;

internal sealed record TelemetrySettings
{
    public const string SectionName = nameof(TelemetrySettings);

    [Required]
    public bool IsEnabled { get; init; } = true;

    [Required, MinLength(1)]
    public required string ServiceName { get; init; }

    [Required, MinLength(1)]
    public required string ServiceVersion { get; init; }

    public string? OpenTelemetryEndpoint { get; init; }
}
