using FinnHub.PortfolioManagement.Domain.Aggregates.Enums;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.Entities;

/// <summary>
/// Transaction represents a buy or sell operation of an asset in a portfolio.
/// It's an Entity that belongs to the Portfolio aggregate.
/// </summary>
public sealed class Transaction : Entity
{
    public Guid PortfolioId { get; private set; }
    public AssetSymbol AssetSymbol { get; private set; }
    public TransactionType Type { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money Price { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public Money? CurrentMarketValue { get; private set; }
    public bool IsSettled { get; private set; }

    #region EF Constructor
#pragma warning disable CS8618
    private Transaction() { }
#pragma warning restore CS8618
    #endregion

    private Transaction(
        Guid portfolioId,
        AssetSymbol assetSymbol,
        TransactionType type,
        Quantity quantity,
        Money price,
        DateTimeOffset transactionDate)
    {
        PortfolioId = portfolioId;
        AssetSymbol = assetSymbol;
        Type = type;
        Quantity = quantity;
        Price = price;
        TransactionDate = transactionDate;
        IsSettled = false;
    }

    public static Transaction CreateBuyTransaction(
        Guid portfolioId,
        AssetSymbol assetSymbol,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
        if (quantity.Value <= 0)
            throw new ArgumentException("Quantity must be positive for a buy transaction", nameof(quantity));

        if (price.Value <= 0)
            throw new ArgumentException("Price must be positive for a buy transaction", nameof(price));

        return new Transaction(
            portfolioId,
            assetSymbol,
            TransactionType.Buy,
            quantity,
            price,
            transactionDate ?? DateTimeOffset.UtcNow);
    }

    public static Transaction CreateSellTransaction(
        Guid portfolioId,
        AssetSymbol assetSymbol,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
        if (quantity.Value <= 0)
            throw new ArgumentException("Quantity must be positive for a sell transaction", nameof(quantity));

        if (price.Value <= 0)
            throw new ArgumentException("Price must be positive for a sell transaction", nameof(price));

        return new Transaction(
            portfolioId,
            assetSymbol,
            TransactionType.Sell,
            quantity,
            price,
            transactionDate ?? DateTimeOffset.UtcNow);
    }

    public static Transaction Create(
        Guid portfolioId,
        AssetSymbol assetSymbol,
        TransactionType type,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
        return type switch
        {
            TransactionType.Buy => CreateBuyTransaction(portfolioId, assetSymbol, quantity, price, transactionDate),
            TransactionType.Sell => CreateSellTransaction(portfolioId, assetSymbol, quantity, price, transactionDate),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Transaction type must be Buy or Sell")
        };
    }

    public static Transaction Create(
        Guid portfolioId,
        string assetSymbol,
        TransactionType type,
        int quantity,
        decimal price,
        DateTimeOffset? transactionDate = null)
    {
        return Create(
            portfolioId,
            AssetSymbol.Create(assetSymbol),
            type,
            Quantity.Create(quantity),
            Money.Create(price),
            transactionDate);
    }

    public void MarkAsSettled()
    {
        if (IsSettled)
            return;

        IsSettled = true;
    }

    // Update with current market value for profit/loss tracking
    public void UpdateMarketValue(Money currentPrice)
    {
        if (currentPrice.Value <= 0)
            throw new ArgumentException("Current price must be positive", nameof(currentPrice));

        CurrentMarketValue = Money.Create(currentPrice.Value * Quantity.Value);
    }

    // Calculate profit/loss if market value is known
    public Money? ProfitLoss
    {
        get
        {
            if (CurrentMarketValue == null || Type != TransactionType.Buy)
                return null;

            return Money.Create(CurrentMarketValue.Value - TotalAmount.Value);
        }
    }

    // Calculate profit/loss percentage if market value is known
    public decimal? ProfitLossPercentage
    {
        get
        {
            if (ProfitLoss == null || TotalAmount.Value == 0)
                return null;

            return ProfitLoss.Value / TotalAmount.Value * 100;
        }
    }

    // Total cost/proceeds of the transaction
    public Money TotalAmount => Money.Create(Price.Value * Quantity.Value);

    // Trading impact methods

    // Apply this transaction to a position (returns updated or new position)
    public Position ApplyToPosition(Position? existingPosition = null)
    {
        if (existingPosition != null && existingPosition.AssetSymbol.Value != AssetSymbol.Value)
            throw new InvalidOperationException("Cannot apply transaction to a position with different asset symbol");

        if (Type == TransactionType.Buy)
            return ApplyBuyTransaction(existingPosition);
        else
            return ApplySellTransaction(existingPosition);
    }

    private Position ApplyBuyTransaction(Position? existingPosition)
    {
        if (existingPosition == null)
        {
            return Position.Create(AssetSymbol, Quantity, Price);
        }

        var totalCostBefore = existingPosition.AverageCost.Value * existingPosition.Quantity.Value;
        var totalCostNew = Price.Value * Quantity.Value;
        var totalQuantity = existingPosition.Quantity.Value + Quantity.Value;
        var newAverageCost = (totalCostBefore + totalCostNew) / totalQuantity;

        return Position.Create(
            AssetSymbol,
            Quantity.Create(totalQuantity),
            Money.Create(newAverageCost));
    }

    private Position ApplySellTransaction(Position? existingPosition)
    {
        if (existingPosition == null)
            throw new InvalidOperationException("Cannot sell an asset that is not in the portfolio");

        if (existingPosition.Quantity.Value < Quantity.Value)
            throw new InvalidOperationException("Cannot sell more than current position quantity");

        var remainingQuantity = existingPosition.Quantity.Value - Quantity.Value;

        // If sold everything, return null or empty position
        if (remainingQuantity == 0)
            return Position.Create(AssetSymbol, Quantity.Create(0), existingPosition.AverageCost);

        return Position.Create(
            AssetSymbol,
            Quantity.Create(remainingQuantity),
            existingPosition.AverageCost);
    }
}