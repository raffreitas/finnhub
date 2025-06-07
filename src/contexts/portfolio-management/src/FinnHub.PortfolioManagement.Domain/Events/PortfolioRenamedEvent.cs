using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Events;

public record PortfolioRenamedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public string NewName { get; init; }
    
    public PortfolioRenamedEvent(Guid portfolioId, string newName)
    {
        PortfolioId = portfolioId;
        NewName = newName;
    }
}
