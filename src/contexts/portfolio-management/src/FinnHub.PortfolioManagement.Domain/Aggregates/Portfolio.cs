using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.Enums;
using FinnHub.PortfolioManagement.Domain.Aggregates.Events;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Aggregates;

/// <summary>
/// Portfolio is the aggregate root for the portfolio management domain.
/// It encapsulates all operations related to positions and transactions.
/// </summary>
public sealed class Portfolio : AggregateRoot
{
    // Collection of positions in this portfolio
    private readonly List<Position> _positions = [];

    // Collection of all transactions for this portfolio
    private readonly List<Transaction> _transactions = [];

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreationDate { get; private set; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<Position> Positions => _positions.AsReadOnly();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    private Portfolio(Guid userId, string name, string? description = null)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }

    public static Portfolio Create(Guid userId, string name, string? description = null)
    {
        var portfolio = new Portfolio(userId, name, description);

        // Add domain event for portfolio creation
        portfolio.AddDomainEvent(new PortfolioCreatedEvent(portfolio.Id, userId, name));

        return portfolio;
    }

    // Process an existing transaction object
    private void ProcessTransaction(Transaction transaction)
    {
        if (transaction.PortfolioId != Id)
            throw new ArgumentException("Transaction belongs to a different portfolio", nameof(transaction));

        // Find existing position for this asset if any
        Position? existingPosition = FindPositionBySymbol(transaction.AssetSymbol.Value);

        // Apply transaction to update or create position
        Position updatedPosition = transaction.ApplyToPosition(existingPosition);

        // Update positions collection
        if (existingPosition != null)
        {
            if (updatedPosition.Quantity.Value > 0)
            {
                // Update existing position
                _positions.Remove(existingPosition);
                _positions.Add(updatedPosition);
            }
            else
            {
                // Position was completely sold, remove it
                _positions.Remove(existingPosition);
            }
        }
        else if (updatedPosition.Quantity.Value > 0)
        {
            // Add new position
            _positions.Add(updatedPosition);
        }

        // Add transaction to the collection
        _transactions.Add(transaction);

        // Raise appropriate domain event based on transaction type
        if (transaction.Type == TransactionType.Buy)
        {
            AddDomainEvent(new AssetBoughtEvent(
                transaction.Id,
                Id,
                transaction.AssetSymbol.Value,
                transaction.Quantity.Value,
                transaction.Price.Value));
        }
        else
        {
            AddDomainEvent(new AssetSoldEvent(
                transaction.Id,
                Id,
                transaction.AssetSymbol.Value,
                transaction.Quantity.Value,
                transaction.Price.Value));
        }

        // Add position update event
        AddDomainEvent(new PortfolioPositionUpdatedEvent(Id, transaction.AssetSymbol.Value));
    }

    // Buy an asset
    public Transaction BuyAsset(
        AssetSymbol assetSymbol,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
        var transaction = Transaction.CreateBuyTransaction(
            Id,
            assetSymbol,
            quantity,
            price,
            transactionDate);

        ProcessTransaction(transaction);

        return transaction;
    }

    // Buy an asset (convenience overload)
    public Transaction BuyAsset(
        string assetSymbol,
        int quantity,
        decimal price,
        DateTimeOffset? transactionDate = null)
    {
        return BuyAsset(
            AssetSymbol.Create(assetSymbol),
            Quantity.Create(quantity),
            Money.Create(price),
            transactionDate);
    }

    // Sell an asset
    public Transaction SellAsset(
        AssetSymbol assetSymbol,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
        // Verify we have enough of the asset to sell
        var existingPosition = FindPositionBySymbol(assetSymbol.Value);
        if (existingPosition == null || existingPosition.Quantity.Value < quantity.Value)
            throw new InvalidOperationException($"Cannot sell {quantity.Value} of {assetSymbol.Value}: insufficient position");

        var transaction = Transaction.CreateSellTransaction(
            Id,
            assetSymbol,
            quantity,
            price,
            transactionDate);

        ProcessTransaction(transaction);

        return transaction;
    }

    // Sell an asset (convenience overload)
    public Transaction SellAsset(
        string assetSymbol,
        int quantity,
        decimal price,
        DateTimeOffset? transactionDate = null)
    {
        return SellAsset(
            AssetSymbol.Create(assetSymbol),
            Quantity.Create(quantity),
            Money.Create(price),
            transactionDate);
    }

    // Find a position by asset symbol
    public Position? FindPositionBySymbol(string assetSymbol)
    {
        return _positions.FirstOrDefault(p => p.AssetSymbol.Value.Equals(assetSymbol, StringComparison.OrdinalIgnoreCase));
    }

    // Get all transactions for a specific asset
    public IReadOnlyList<Transaction> GetTransactionsForAsset(string assetSymbol)
    {
        return _transactions
            .Where(t => t.AssetSymbol.Value.Equals(assetSymbol, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsReadOnly();
    }

    // Calculate total portfolio value based on each position's value
    public Money CalculateCurrentValue()
    {
        decimal totalValue = 0;

        foreach (var position in _positions)
        {
            if (position.CurrentMarketValue != null)
            {
                totalValue += position.CurrentMarketValue.Value.Value;
            }
            else
            {
                totalValue += position.TotalCost.Value;
            }
        }

        return Money.Create(totalValue);
    }

    // Calculate total cost basis of the portfolio
    public Money CalculateTotalCostBasis()
    {
        decimal totalCost = 0;

        foreach (var position in _positions)
        {
            totalCost += position.TotalCost.Value;
        }

        return Money.Create(totalCost);
    }

    // Update all positions with current market prices
    public void UpdatePositionsMarketValue(Dictionary<string, decimal> currentPrices)
    {
        foreach (var position in _positions)
        {
            if (currentPrices.TryGetValue(position.AssetSymbol.Value, out decimal currentPrice))
            {
                position.UpdateMarketValue(Money.Create(currentPrice));
            }
        }

        // Add valuation update event
        Money totalValue = CalculateCurrentValue();
        Money costBasis = CalculateTotalCostBasis();
        decimal profitLoss = totalValue.Value - costBasis.Value;
        decimal profitLossPercentage = costBasis.Value != 0 ? profitLoss / costBasis.Value * 100 : 0;

        AddDomainEvent(new PortfolioValuationUpdatedEvent(Id, totalValue.Value, profitLoss, profitLossPercentage));
    }

    // Rename the portfolio
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Portfolio name cannot be empty", nameof(newName));

        Name = newName;

        AddDomainEvent(new PortfolioRenamedEvent(Id, newName));
    }

    // Update the description
    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    // Delete the portfolio
    public void Delete()
    {
        AddDomainEvent(new PortfolioDeletedEvent(Id));
    }
}
