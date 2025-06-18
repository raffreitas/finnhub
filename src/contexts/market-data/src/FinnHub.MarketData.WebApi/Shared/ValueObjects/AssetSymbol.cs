namespace FinnHub.MarketData.WebApi.Shared.ValueObjects;

public sealed record AssetSymbol
{
    public string Value { get; }

    public AssetSymbol(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Asset symbol cannot be empty", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Asset symbol cannot exceed 20 characters", nameof(value));

        Value = value.ToUpperInvariant();
    }

    public static implicit operator string(AssetSymbol symbol) => symbol.Value;
    public static implicit operator AssetSymbol(string value) => new(value);
}
