using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Events;

public record PortfolioDeletedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    
    public PortfolioDeletedEvent(Guid portfolioId)
    {
        PortfolioId = portfolioId;
    }
}