using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

using FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Settings;
using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Context;

using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

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
    private readonly ActivitySource _activitySource = new("FinnHub.MarketData.Messaging");
    private readonly TextMapPropagator _propagator = Propagators.DefaultTextMapPropagator;

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
        using var activity = _activitySource.StartActivity($"publish {typeof(T).Name}");

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
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Headers = new Dictionary<string, object?>()
        };

        var propagationContext = new PropagationContext(
            activity?.Context ?? Activity.Current?.Context ?? default,
            Baggage.Current
        );

        _propagator.Inject(propagationContext, properties.Headers, (headers, key, value) => headers![key] = value);

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", messageSettings.ExchangeName);
        activity?.SetTag("messaging.destination_kind", "exchange");
        activity?.SetTag("messaging.operation", "publish");
        activity?.SetTag("messaging.message_type", messageType);

        await channel.BasicPublishAsync(
            exchange: messageSettings.ExchangeName,
            routingKey: messageSettings.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }

    public async Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
    {
        await EnsureInfrastructureAsync<T>(cancellationToken);

        var messageType = typeof(T).Name;
        var messageConfig = _settings.GetMessageSettings(messageType);

        var channel = await _connectionManager.CreateChannelAsync(cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs @event) =>
        {
            // Extract trace context from message headers
            var parentContext = _propagator.Extract(
                default,
                @event.BasicProperties.Headers ?? new Dictionary<string, object?>(),
                (headers, key) => headers.TryGetValue(key, out var value) 
                    ? [value?.ToString() ?? string.Empty] 
                    : Array.Empty<string>());

            using var activity = _activitySource.StartActivity(
                $"consume {messageType}",
                ActivityKind.Consumer,
                parentContext.ActivityContext
            );

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination", messageConfig.QueueName);
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.operation", "consume");
            activity?.SetTag("messaging.message_type", messageType);
            activity?.SetTag("messaging.correlation_id", @event.BasicProperties.CorrelationId);

            try
            {
                var jsonMessage = Encoding.UTF8.GetString(@event.Body.Span);
                var message = JsonSerializer.Deserialize<T>(jsonMessage);

                if (message is not null)
                {
                    await handler(message, cancellationToken);
                    await channel.BasicAckAsync(@event.DeliveryTag, false, cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize message of type {MessageType}", messageType);
                    await channel.BasicRejectAsync(@event.DeliveryTag, false, cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Error, "Failed to deserialize message");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message of type {MessageType}", messageType);
                await channel.BasicRejectAsync(@event.DeliveryTag, requeue: false, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddException(ex);
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
        _activitySource?.Dispose();
        _initSemaphore?.Dispose();
        _connectionManager?.Dispose();
    }
}
