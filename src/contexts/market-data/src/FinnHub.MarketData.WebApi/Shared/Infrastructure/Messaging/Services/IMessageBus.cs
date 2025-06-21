namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken cancellationToken = default);
    Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken = default);
}
