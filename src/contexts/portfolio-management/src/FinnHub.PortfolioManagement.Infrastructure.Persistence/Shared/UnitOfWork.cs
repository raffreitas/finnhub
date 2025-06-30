using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Infrastructure.Messaging.Models;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Shared;

internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var messages = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .SelectMany(e =>
            {
                IEnumerable<DomainEvent> domainEvents = [.. e.Entity.DomainEvents];
                e.Entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(x => new OutboxMessage
            {
                EventName = x.GetType().Name,
                MessageType = "DomainEvent",
                MessageContent = x,
            })
            .ToList();

        await dbContext.Set<OutboxMessage>().AddRangeAsync(messages, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
