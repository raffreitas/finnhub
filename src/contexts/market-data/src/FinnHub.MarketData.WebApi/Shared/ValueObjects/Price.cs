namespace FinnHub.MarketData.WebApi.Shared.ValueObjects;

public sealed record Price
{
    public decimal Value { get; }

    public Price(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Price cannot be negative", nameof(value));

        Value = Math.Round(value, 8);
    }

    public Price Add(Price other) => new(Value + other.Value);
    public Price Subtract(Price other) => new(Value - other.Value);
    public Price Multiply(decimal multiplier) => new(Value * multiplier);

    public static implicit operator decimal(Price price) => price.Value;
    public static implicit operator Price(decimal value) => new(value);
}
