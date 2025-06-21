using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record PortfolioCreatedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    protected override string EventVersion => "1.0.0";

    public PortfolioCreatedEvent(Guid portfolioId, Guid userId, string name) : base(portfolioId)
    {
        PortfolioId = portfolioId;
        UserId = userId;
        Name = name;
    }
}
