using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record AssetSoldEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid PortfolioId { get; init; }
    public string AssetSymbol { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }

    protected override string EventVersion => "1.0.0";

    public AssetSoldEvent(Guid transactionId, Guid portfolioId, string assetSymbol, int quantity, decimal price) : base(portfolioId)
    {
        TransactionId = transactionId;
        PortfolioId = portfolioId;
        AssetSymbol = assetSymbol;
        Quantity = quantity;
        Price = price;
    }
}
