namespace FinnHub.MarketData.WebApi.Shared.ValueObjects;

public sealed record Timestamp
{
    public DateTime Value { get; }

    public Timestamp(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            value = value.ToUniversalTime();

        Value = value;
    }

    public bool IsRecent(TimeSpan threshold) =>
        DateTime.UtcNow - Value <= threshold;

    public static implicit operator DateTime(Timestamp timestamp) => timestamp.Value;
    public static implicit operator Timestamp(DateTime value) => new(value);

    public static Timestamp Now => new(DateTime.UtcNow);
}
