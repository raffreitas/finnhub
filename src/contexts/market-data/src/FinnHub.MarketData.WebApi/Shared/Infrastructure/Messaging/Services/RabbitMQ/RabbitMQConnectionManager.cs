using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Messaging.Services.RabbitMQ;

internal sealed class RabbitMQConnectionManager(
    IConnectionFactory connectionFactory,
    ILogger<RabbitMQConnectionManager> logger
) : IDisposable
{
    private IConnection? _connection;
    private bool _disposed;

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        => IsConnected
            ? await _connection!.CreateChannelAsync(cancellationToken: cancellationToken)
            : throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");

    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
    {
        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        if (IsConnected)
        {
            _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
            _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
            _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;
            return true;
        }

        logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opented.");
        return false;
    }

    private async Task OnConnectionShutdownAsync(object? sender, ShutdownEventArgs args)
    {
        if (_disposed) return;
        logger.LogWarning(args.Exception, "A RabbitMQ connection is on shutdown. Trying to re-connect...");
        _ = await TryConnectAsync();
    }

    private async Task OnCallbackExceptionAsync(object? sender, CallbackExceptionEventArgs args)
    {
        if (_disposed) return;
        logger.LogWarning(args.Exception, "A RabbitMQ connection throw exception. Trying to re-connect...");
        _ = await TryConnectAsync();
    }

    private async Task OnConnectionBlockedAsync(object? sender, ConnectionBlockedEventArgs args)
    {
        if (_disposed) return;
        logger.LogWarning("A RabbitMQ connection is blocked by reason {Reason}. Trying to re-connect...", args.Reason);
        _ = await TryConnectAsync();
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _connection?.Dispose();
            _disposed = true;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error disposing RabbitMQ connection");
        }
    }
}
