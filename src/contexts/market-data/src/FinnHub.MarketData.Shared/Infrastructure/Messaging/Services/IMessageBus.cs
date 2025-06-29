namespace FinnHub.MarketData.Shared.Infrastructure.Messaging.Services;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken cancellationToken = default);
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default);
}
