using System.ComponentModel.DataAnnotations;

namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Settings;

public sealed record EventBusSettings
{
    public const string SectionName = nameof(EventBusSettings);

    public Dictionary<string, EventRouting> EventRoutings { get; set; } = [];

    public EventRouting GetEventRouting(string eventName)
    {
        if (EventRoutings.TryGetValue(eventName, out var routing))
            return routing;

        throw new KeyNotFoundException($"Routing for event '{eventName}' is not configured.");
    }
}

public sealed record EventRouting
{
    [Required, MinLength(1)]
    public required string Exchange { get; set; }

    [Required, MinLength(1)]
    public required string RoutingKey { get; set; }
}
