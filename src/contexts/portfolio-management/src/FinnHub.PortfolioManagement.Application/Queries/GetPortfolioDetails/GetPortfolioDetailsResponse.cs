namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfolioDetails;

public sealed record GetPortfolioDetailsResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public required Guid UserId { get; init; }
    public required decimal TotalValue { get; init; }
    public required decimal TotalInvested { get; init; }
    public required decimal TotalUnrealizedPnL { get; init; }
    public required decimal TotalUnrealizedPnLPercentage { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<PositionResponse> Positions { get; init; } = [];
}


public sealed record PositionResponse
{
    public required Guid Id { get; init; }
    public required string Symbol { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal AveragePrice { get; init; }
    public required decimal TotalCost { get; init; }
    public decimal? CurrentMarketPrice { get; init; }
    public decimal? CurrentMarketValue { get; init; }
    public decimal? UnrealizedPnL { get; init; }
    public decimal? UnrealizedPnLPercentage { get; init; }
}
