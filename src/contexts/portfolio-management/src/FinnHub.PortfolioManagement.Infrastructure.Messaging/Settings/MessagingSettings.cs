using System.ComponentModel.DataAnnotations;

namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;
internal sealed record MessagingSettings
{
    public const string SectionName = nameof(MessagingSettings);

    [Required] public string HostName { get; set; } = "localhost";
    [Required] public int Port { get; set; } = 5672;
    [Required] public string UserName { get; set; } = "guest";
    [Required] public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int RetryCount { get; set; } = 5;
    public int RetryIntervalMilliseconds { get; set; } = 200;
    public bool PersistentDeliveryMode { get; set; } = true;
}
