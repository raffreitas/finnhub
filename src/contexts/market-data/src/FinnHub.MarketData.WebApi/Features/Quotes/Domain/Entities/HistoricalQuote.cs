using FinnHub.MarketData.WebApi.Shared.Domain.Entities;

namespace FinnHub.MarketData.WebApi.Features.Quotes.Domain.Entities;

public sealed class HistoricalQuote : Entity
{
    public string AssetSymbol { get; private set; }
    public DateOnly Date { get; private set; }
    public decimal Open { get; private set; }
    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Close { get; private set; }
    public long Volume { get; private set; }
    public string Interval { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public HistoricalQuote(
        string assetSymbol,
        DateOnly date,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        long volume,
        string interval
    )
    {
        AssetSymbol = assetSymbol;
        Date = date;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        Interval = interval;
    }
}
