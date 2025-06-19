using FinnHub.MarketData.WebApi.Features.Quotes.Events;
using FinnHub.MarketData.WebApi.Shared.Domain.Abstractions;
using FinnHub.MarketData.WebApi.Shared.Domain.ValueObjects;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Entities;

public class HistoricalQuote : IAggregateRoot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public AssetSymbol AssetSymbol { get; private set; }
    public DateOnly Date { get; private set; }
    public OHLCV Data { get; private set; }
    public string Interval { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private HistoricalQuote() { }

    public HistoricalQuote(
        AssetSymbol assetSymbol,
        DateOnly date,
        OHLCV data,
        string interval = "1d")
    {
        AssetSymbol = assetSymbol ?? throw new ArgumentNullException(nameof(assetSymbol));
        Date = date;
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Interval = interval ?? throw new ArgumentNullException(nameof(interval));
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new NewHistoricalQuoteAvailableEvent(
            AssetSymbol.Value,
            Date,
            Data.Close.Value,
            Interval));
    }

    public void UpdateData(OHLCV newData)
    {
        ArgumentNullException.ThrowIfNull(newData);

        Data = newData;
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
