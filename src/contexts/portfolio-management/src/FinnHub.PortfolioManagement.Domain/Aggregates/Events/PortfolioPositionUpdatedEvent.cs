using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record PortfolioPositionUpdatedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public string AssetSymbol { get; init; }
    protected override string EventVersion => "1.0.0";
    public PortfolioPositionUpdatedEvent(Guid portfolioId, string assetSymbol) : base(portfolioId)
    {
        PortfolioId = portfolioId;
        AssetSymbol = assetSymbol;
    }
}
