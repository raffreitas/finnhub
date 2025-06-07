using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Events;

public record PortfolioCreatedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }

    public PortfolioCreatedEvent(Guid portfolioId, Guid userId, string name)
    {
        PortfolioId = portfolioId;
        UserId = userId;
        Name = name;
    }
}
