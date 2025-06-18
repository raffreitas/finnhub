using FinnHub.MarketData.WebApi.Features.Quotes.Events;
using FinnHub.MarketData.WebApi.Shared.Abstractions;
using FinnHub.MarketData.WebApi.Shared.ValueObjects;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Entities;

public class RealtimeQuote : IAggregateRoot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public AssetSymbol AssetSymbol { get; private set; }
    public Price Price { get; private set; }
    public Timestamp Timestamp { get; private set; }
    public long? Volume { get; private set; }
    public Price? Change { get; private set; }
    public decimal? ChangePercent { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private RealtimeQuote() { }

    public RealtimeQuote(
        AssetSymbol assetSymbol,
        Price price,
        Timestamp timestamp,
        long? volume = null,
        Price? previousPrice = null)
    {
        AssetSymbol = assetSymbol ?? throw new ArgumentNullException(nameof(assetSymbol));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Timestamp = timestamp ?? throw new ArgumentNullException(nameof(timestamp));
        Volume = volume;

        if (previousPrice != null)
        {
            Change = Price.Subtract(previousPrice);
            ChangePercent = previousPrice.Value != 0
                ? (Price.Value - previousPrice.Value) / previousPrice.Value * 100
                : 0;
        }

        AddDomainEvent(new RealtimeQuoteUpdatedEvent(
            AssetSymbol.Value,
            Price.Value,
            Timestamp.Value,
            Volume));
    }

    public void UpdatePrice(Price newPrice, Timestamp timestamp, long? volume = null)
    {
        var previousPrice = Price;
        Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        Timestamp = timestamp ?? throw new ArgumentNullException(nameof(timestamp));
        Volume = volume;

        Change = Price.Subtract(previousPrice);
        ChangePercent = previousPrice.Value != 0
            ? (Price.Value - previousPrice.Value) / previousPrice.Value * 100
            : 0;

        AddDomainEvent(new RealtimeQuoteUpdatedEvent(
            AssetSymbol.Value,
            Price.Value,
            Timestamp.Value,
            Volume));
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
