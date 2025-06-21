using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

using FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Settings;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Context;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services.RabbitMQ;

internal sealed class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private readonly MessagingSettings _settings;
    private readonly RabbitMQConnectionManager _connectionManager;
    private readonly ILogger<RabbitMQMessageBus> _logger;
    private readonly ICorrelationContextAccessor _correlationContextAccessor;
    private readonly ConcurrentDictionary<string, bool> _initializedInfrastructure = new();
    private readonly SemaphoreSlim _initSemaphore = new(1, 1);

    public RabbitMQMessageBus(
        RabbitMQConnectionManager connectionManager,
        IOptions<MessagingSettings> options,
        ILogger<RabbitMQMessageBus> logger,
        ICorrelationContextAccessor correlationContextAccessor
    )
    {
        _settings = options.Value;
        _connectionManager = connectionManager;
        _logger = logger;
        _correlationContextAccessor = correlationContextAccessor;
    }

    private async Task EnsureInfrastructureAsync<T>(CancellationToken cancellationToken = default)
    {
        var messageType = typeof(T).Name;

        if (_initializedInfrastructure.ContainsKey(messageType))
            return;

        await _initSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_initializedInfrastructure.ContainsKey(messageType))
                return;

            if (!_connectionManager.IsConnected)
            {
                await _connectionManager.TryConnectAsync(cancellationToken);
            }

            var config = _settings.GetMessageSettings(typeof(T).Name);

            await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: config.ExchangeName,
                type: config.ExchangeType,
                durable: config.Durable,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await channel.QueueDeclareAsync(
                queue: config.QueueName,
                durable: config.Durable,
                exclusive: config.Exclusive,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await channel.QueueBindAsync(
                queue: config.QueueName,
                exchange: config.ExchangeName,
                routingKey: config.RoutingKey,
                arguments: null,
                cancellationToken: cancellationToken
            );

            _initializedInfrastructure.TryAdd(messageType, true);

            _logger.LogInformation("Infrastructure initialized for {MessageType} - Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}",
                messageType, config.ExchangeName, config.QueueName, config.RoutingKey);
        }
        finally
        {
            _initSemaphore.Release();
        }
    }

    public async Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync<T>(cancellationToken);

        var messageType = typeof(T).Name;
        var messageSettings = _settings.GetMessageSettings(messageType);

        await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            CorrelationId = _correlationContextAccessor?.Context?.CorrelationId ?? Guid.NewGuid().ToString(),
            MessageId = Guid.NewGuid().ToString(),
            Type = messageType,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        await channel.BasicPublishAsync(
            exchange: messageSettings.ExchangeName,
            routingKey: messageSettings.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync<T>(cancellationToken);

        var messageType = typeof(T).Name;
        var messageConfig = _settings.GetMessageSettings(messageType);

        await using var channel = await _connectionManager.CreateChannelAsync(cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs @event) =>
        {
            try
            {
                var jsonMessage = Encoding.UTF8.GetString(@event.Body.Span);
                var message = JsonSerializer.Deserialize<T>(jsonMessage);

                if (message is not null)
                {
                    await handler(message);
                    await channel.BasicAckAsync(@event.DeliveryTag, false, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize message of type {MessageType}", messageType);
                    await channel.BasicRejectAsync(@event.DeliveryTag, false, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message of type {MessageType}", messageType);
                await channel.BasicRejectAsync(@event.DeliveryTag, requeue: false, cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: messageConfig.QueueName,
            autoAck: false,
            consumer,
            cancellationToken
        );
        _logger.LogInformation("Subscribed to queue {QueueName} for message type {MessageType}", messageConfig.QueueName, messageType);
    }

    public void Dispose()
    {
        _initSemaphore?.Dispose();
        _connectionManager?.Dispose();
    }
}
