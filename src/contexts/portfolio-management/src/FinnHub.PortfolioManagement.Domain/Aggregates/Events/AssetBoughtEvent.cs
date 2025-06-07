using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Events;

public record AssetBoughtEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid PortfolioId { get; init; }
    public string AssetSymbol { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    
    public AssetBoughtEvent(Guid transactionId, Guid portfolioId, string assetSymbol, int quantity, decimal price)
    {
        TransactionId = transactionId;
        PortfolioId = portfolioId;
        AssetSymbol = assetSymbol;
        Quantity = quantity;
        Price = price;
    }
}
