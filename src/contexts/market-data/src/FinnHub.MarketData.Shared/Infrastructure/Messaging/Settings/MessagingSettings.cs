using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

namespace FinnHub.MarketData.Shared.Infrastructure.Messaging.Settings;

internal sealed record MessagingSettings
{
    public const string SectionName = nameof(MessagingSettings);

    [Required, MinLength(1)]
    public required string ConnectionString { get; init; }

    [Required, MinLength(1), ValidateEnumeratedItems]
    public required Dictionary<string, MessageSettings> Messages { get; init; } = [];

    public MessageSettings GetMessageSettings(string messageKey)
    {
        return Messages.TryGetValue(messageKey, out MessageSettings? value)
            ? value
            : throw new InvalidOperationException();
    }
}


internal sealed record MessageSettings
{
    [Required, MinLength(1)]
    public required string ExchangeName { get; init; }

    [Required, MinLength(1)]
    public required string ExchangeType { get; init; }

    [Required, MinLength(1)]
    public required string RoutingKey { get; init; }

    [Required, MinLength(1)]
    public required string QueueName { get; init; }

    public required bool Exclusive { get; init; } = false;

    public required bool Durable { get; init; } = true;
}
