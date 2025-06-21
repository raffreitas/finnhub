using FinnHub.Shared.Kernel;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;

public record AssetSymbol : IValueObject
{
    public string Value { get; private init; }

    private AssetSymbol(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Asset symbol cannot be empty", nameof(value));

        if (value.Length > 10)
            throw new ArgumentException("Asset symbol cannot exceed 10 characters", nameof(value));

        Value = value.ToUpperInvariant();
    }

    public static AssetSymbol Create(string value)
        => new(value);

    public override string ToString() => Value;

    public static implicit operator string(AssetSymbol symbol) => symbol.Value;
}