using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.Enums;
using FinnHub.PortfolioManagement.Domain.Aggregates.Events;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates;

public sealed class Portfolio : AggregateRoot
{
    private readonly List<Position> _positions = [];
    private readonly List<Transaction> _transactions = [];

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreationDate { get; private set; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<Position> Positions => _positions.AsReadOnly();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    #region EF Constructor
#pragma warning disable CS8618
    private Portfolio() { }
#pragma warning restore CS8618
    #endregion

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


    #region BuyAsset Methods
    public Transaction BuyAsset(
       AssetSymbol assetSymbol,
       Quantity quantity,
       Money price,
       Money currentMarketValue,
       DateTimeOffset? transactionDate = null)
    {
        var transaction = Transaction.CreateBuyTransaction(
            Id,
            assetSymbol,
            quantity,
            price,
            currentMarketValue,
            transactionDate);

        ProcessTransaction(transaction);

        return transaction;
    }

    public Transaction BuyAsset(
        string assetSymbol,
        int quantity,
        decimal price,
        decimal currentMarketValue,
        DateTimeOffset? transactionDate = null)
    {
        return BuyAsset(
            AssetSymbol.Create(assetSymbol),
            Quantity.Create(quantity),
            Money.Create(price),
            Money.Create(currentMarketValue),
            transactionDate
        );
    }
    #endregion


    #region SellAsset Methods
    public Transaction SellAsset(
        AssetSymbol assetSymbol,
        Quantity quantity,
        Money price,
        DateTimeOffset? transactionDate = null)
    {
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
    #endregion

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

    public Position? FindPositionBySymbol(string assetSymbol)
    {
        return _positions.FirstOrDefault(p => p.AssetSymbol.Value.Equals(assetSymbol, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<Transaction> GetTransactionsForAsset(string assetSymbol)
    {
        return _transactions
            .Where(t => t.AssetSymbol.Value.Equals(assetSymbol, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsReadOnly();
    }

    public Money CalculateCurrentValue()
    {
        decimal totalValue = 0;

        foreach (var position in _positions)
        {
            if (position.CurrentMarketValue != null)
            {
                totalValue += position.CurrentMarketValue.Value;
            }
            else
            {
                totalValue += position.TotalCost.Value;
            }
        }

        return Money.Create(totalValue);
    }

    public Money CalculateTotalCostBasis()
    {
        decimal totalCost = 0;

        foreach (var position in _positions)
        {
            totalCost += position.TotalCost.Value;
        }

        return Money.Create(totalCost);
    }

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

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Portfolio name cannot be empty", nameof(newName));

        Name = newName;

        AddDomainEvent(new PortfolioRenamedEvent(Id, newName));
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    public void Delete()
    {
        AddDomainEvent(new PortfolioDeletedEvent(Id));
    }
}
