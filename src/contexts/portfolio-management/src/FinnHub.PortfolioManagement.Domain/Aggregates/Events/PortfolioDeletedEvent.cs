using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record PortfolioDeletedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    protected override string EventVersion => "1.0.0";


    public PortfolioDeletedEvent(Guid portfolioId) : base(portfolioId)
    {
        PortfolioId = portfolioId;
    }
}