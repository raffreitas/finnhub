namespace FinnHub.PortfolioManagement.Application.DTOs;

public sealed class PortfolioTransactionsResponseDto
{
    public Guid Id { get; init; }
    public Guid PortfolioId { get; init; }
    public string AssetSymbol { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal CurrentMarketValue { get; init; }
    public DateTime TransactionDate { get; init; }
    public bool IsSettled { get; init; }
    public decimal? ProfitLoss { get; init; }
    public decimal? ProfitLossPercentage { get; init; }
}
