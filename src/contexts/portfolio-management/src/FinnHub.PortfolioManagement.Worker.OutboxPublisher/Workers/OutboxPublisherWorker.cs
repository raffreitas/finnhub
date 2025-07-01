using FinnHub.PortfolioManagement.Application.Abstractions.Messaging;
using FinnHub.PortfolioManagement.Infrastructure.Messaging.Models;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Worker.OutboxPublisher.Workers;

public class OutboxPublisherWorker(
    IServiceProvider serviceProvider,
    ILogger<OutboxPublisherWorker> logger
) : BackgroundService
{
    private const int DelaySeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var pendingMessages = await dbContext.Set<OutboxMessage>()
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in pendingMessages)
                {
                    try
                    {
                        var messageContent = message.MessageContent.ToNameTypeObject<object>();
                        var eventType = messageContent.GetType()
                            ?? throw new InvalidOperationException($"Event type '{messageContent.GetType().Name}' could not be resolved.");
                        var publishMethod = (typeof(IMessageBus).GetMethod("PublishAsync")?.MakeGenericMethod(eventType))
                            ?? throw new InvalidOperationException("PublishAsync method could not be resolved.");
                        await (Task)publishMethod.Invoke(messageBus, [messageContent, null, stoppingToken])!;
                        message.ProcessedAt = DateTimeOffset.UtcNow;
                        message.ErrorMessage = null;
                    }
                    catch (Exception ex)
                    {
                        message.DeliveryAttempts++;
                        message.ErrorMessage = ex.Message;
                        logger.LogError(ex, "Error publishing outbox message id: {MessageId}", message.Id);
                    }
                }

                if (pendingMessages.Count > 0)
                    await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on processing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), stoppingToken);
        }
    }
}
