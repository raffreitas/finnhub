namespace FinnHub.PortfolioManagement.Infrastructure.Messaging.Models;

public sealed class OutboxMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public required string MessageType { get; set; }
    public required object MessageContent { get; set; }
    public object? Headers { get; set; }
    public required int DeliveryAttempts { get; set; }
    public string? ErrorMessage { get; set; }
}