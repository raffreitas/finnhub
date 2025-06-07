using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Events;

public record PortfolioPositionUpdatedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public string AssetSymbol { get; init; }
    
    public PortfolioPositionUpdatedEvent(Guid portfolioId, string assetSymbol)
    {
        PortfolioId = portfolioId;
        AssetSymbol = assetSymbol;
    }
}
