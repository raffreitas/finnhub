namespace FinnHub.PortfolioManagement.Domain.SeedWork;

public abstract record DomainEvent
{
    public DateTimeOffset OccuredOn { get; protected set; } = DateTimeOffset.UtcNow;
}
