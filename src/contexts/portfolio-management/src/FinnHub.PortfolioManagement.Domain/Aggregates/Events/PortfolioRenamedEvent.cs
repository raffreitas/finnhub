using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record PortfolioRenamedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public string NewName { get; init; }
    protected override string EventVersion => "1.0.0";

    public PortfolioRenamedEvent(Guid portfolioId, string newName) : base(portfolioId)
    {
        PortfolioId = portfolioId;
        NewName = newName;
    }
}
