namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;

internal sealed record OutboxSettings
{
    public const string SectionName = nameof(OutboxSettings);
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(10);
    public int BatchSize { get; set; } = 50;
    public int MaxDeliveryAttempts { get; set; } = 3;
    public bool Enabled { get; set; } = true;
}