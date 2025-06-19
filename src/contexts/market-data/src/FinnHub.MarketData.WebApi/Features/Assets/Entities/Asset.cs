using FinnHub.MarketData.WebApi.Features.Assets.Events;
using FinnHub.MarketData.WebApi.Shared.Domain.Abstractions;
using FinnHub.MarketData.WebApi.Shared.Domain.Enums;
using FinnHub.MarketData.WebApi.Shared.Domain.ValueObjects;

namespace FinnHub.MarketData.WebApi.Features.Assets.Entities;

public class Asset : IAggregateRoot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public AssetSymbol Symbol { get; private set; }
    public string Name { get; private set; }
    public AssetType Type { get; private set; }
    public string Exchange { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Asset() { }

    public Asset(AssetSymbol symbol, string name, AssetType type, string exchange)
    {
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new AssetListedEvent(Symbol.Value, Name, Type.ToString(), Exchange));
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Asset name cannot be empty", nameof(newName));

        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    void IAggregateRoot.ClearDomainEvents()
    {
        throw new NotImplementedException();
    }
}
