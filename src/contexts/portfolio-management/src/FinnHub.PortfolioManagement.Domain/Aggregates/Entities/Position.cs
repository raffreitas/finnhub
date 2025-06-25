using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Entities;

public sealed class Position : Entity
{
    public AssetSymbol AssetSymbol { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money AverageCost { get; private set; }
    public Money CurrentMarketPrice { get; private set; }
    public DateTimeOffset LastUpdated { get; private set; }
    public Guid PortfolioId { get; }

    #region EF Constructor
#pragma warning disable CS8618
    private Position() { }
#pragma warning restore CS8618
    #endregion

    private Position(AssetSymbol assetSymbol, Quantity quantity, Money averageCost, Money currentPrice)
    {
        AssetSymbol = assetSymbol;
        Quantity = quantity;
        AverageCost = averageCost;
        CurrentMarketPrice = currentPrice;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    public static Position Create(AssetSymbol assetSymbol, Quantity quantity, Money averageCost, Money currentPrice)
    {
        if (quantity.Value < 0)
            throw new ArgumentException("Position quantity cannot be negative", nameof(quantity));

        if (averageCost.Value < 0)
            throw new ArgumentException("Position average cost cannot be negative", nameof(averageCost));

        if (currentPrice.Value < 0)
            throw new ArgumentException("Current market price cannot be negative", nameof(currentPrice));

        return new Position(assetSymbol, quantity, averageCost, currentPrice);
    }

    public static Position Create(string assetSymbol, int quantity, decimal averageCost, decimal currentPrice)
    {
        return new Position(
            AssetSymbol.Create(assetSymbol),
            Quantity.Create(quantity),
            Money.Create(averageCost),
            Money.Create(currentPrice));
    }

    public void UpdateMarketValue(Money currentPrice)
    {
        if (currentPrice.Value <= 0)
            throw new ArgumentException("Current price must be positive", nameof(currentPrice));

        CurrentMarketPrice = currentPrice;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    public Money TotalCost => Money.Create(AverageCost.Value * Quantity.Value);

    public Money CurrentMarketValue => Money.Create(CurrentMarketPrice.Value * Quantity.Value);

    public Money UnrealizedProfitLoss => Money.Create(CurrentMarketValue.Value - TotalCost.Value);

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
