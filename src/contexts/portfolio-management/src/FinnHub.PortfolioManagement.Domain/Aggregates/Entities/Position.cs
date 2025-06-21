using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Entities;

/// <summary>
/// Position represents the current holdings of a specific asset within a portfolio.
/// It tracks quantity, average cost, and current market value.
/// </summary>
public sealed class Position : Entity
{
    public AssetSymbol AssetSymbol { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money AverageCost { get; private set; }
    public Money? CurrentMarketPrice { get; private set; }
    public DateTimeOffset LastUpdated { get; private set; }
    public Guid PortfolioId { get; private set; }

    #region EF Constructor
#pragma warning disable CS8618
    private Position() { }
#pragma warning restore CS8618
    #endregion

    private Position(AssetSymbol assetSymbol, Quantity quantity, Money averageCost)
    {
        AssetSymbol = assetSymbol;
        Quantity = quantity;
        AverageCost = averageCost;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    public static Position Create(AssetSymbol assetSymbol, Quantity quantity, Money averageCost)
    {
        if (quantity.Value < 0)
            throw new ArgumentException("Position quantity cannot be negative", nameof(quantity));

        if (averageCost.Value < 0)
            throw new ArgumentException("Position average cost cannot be negative", nameof(averageCost));

        return new Position(assetSymbol, quantity, averageCost);
    }

    public static Position Create(string assetSymbol, int quantity, decimal averageCost)
    {
        return new Position(
            AssetSymbol.Create(assetSymbol),
            Quantity.Create(quantity),
            Money.Create(averageCost));
    }

    public void UpdateMarketValue(Money currentPrice)
    {
        if (currentPrice.Value <= 0)
            throw new ArgumentException("Current price must be positive", nameof(currentPrice));

        CurrentMarketPrice = currentPrice;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    public Money TotalCost => Money.Create(AverageCost.Value * Quantity.Value);

    public Money? CurrentMarketValue => CurrentMarketPrice is not null
        ? Money.Create(CurrentMarketPrice.Value * Quantity.Value)
        : null;

    public Money? UnrealizedProfitLoss => CurrentMarketValue is not null
        ? Money.Create(CurrentMarketValue.Value - TotalCost.Value)
        : null;

    public decimal? UnrealizedProfitLossPercentage
    {
        get
        {
            if (UnrealizedProfitLoss is null || TotalCost.Value == 0)
                return null;

            return UnrealizedProfitLoss.Value / TotalCost.Value * 100;
        }
    }
}
