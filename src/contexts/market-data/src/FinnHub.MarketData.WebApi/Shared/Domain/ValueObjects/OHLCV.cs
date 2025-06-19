namespace FinnHub.MarketData.WebApi.Shared.Domain.ValueObjects;

public sealed record OHLCV
{
    public Price Open { get; }
    public Price High { get; }
    public Price Low { get; }
    public Price Close { get; }
    public long Volume { get; }

    public OHLCV(Price open, Price high, Price low, Price close, long volume)
    {
        Open = open ?? throw new ArgumentNullException(nameof(open));
        High = high ?? throw new ArgumentNullException(nameof(high));
        Low = low ?? throw new ArgumentNullException(nameof(low));
        Close = close ?? throw new ArgumentNullException(nameof(close));
        Volume = volume;

        ValidateOHLCV();
    }

    private void ValidateOHLCV()
    {
        if (High.Value < Math.Max(Open.Value, Close.Value))
            throw new ArgumentException("High price must be >= max(Open, Close)");

        if (Low.Value > Math.Min(Open.Value, Close.Value))
            throw new ArgumentException("Low price must be <= min(Open, Close)");

        if (Volume < 0)
            throw new ArgumentException("Volume cannot be negative");
    }
}
