namespace FinnHub.PortfolioManagement.Domain.SeedWork;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
