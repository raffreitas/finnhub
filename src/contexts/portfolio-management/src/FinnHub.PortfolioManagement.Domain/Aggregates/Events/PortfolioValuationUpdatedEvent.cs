using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record PortfolioValuationUpdatedEvent : DomainEvent
{
    public Guid PortfolioId { get; init; }
    public decimal TotalValue { get; init; }
    public decimal ProfitLoss { get; init; }
    public decimal ProfitLossPercentage { get; init; }
    public DateTimeOffset ValuationTime { get; init; }

    protected override string EventVersion => "1.0.0";

    public PortfolioValuationUpdatedEvent(
        Guid portfolioId,
        decimal totalValue,
        decimal profitLoss,
        decimal profitLossPercentage
    ) : base(portfolioId)
    {
        PortfolioId = portfolioId;
        TotalValue = totalValue;
        ProfitLoss = profitLoss;
        ProfitLossPercentage = profitLossPercentage;
        ValuationTime = DateTimeOffset.UtcNow;
    }
}