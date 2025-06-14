using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;

public record Quantity : IValueObject
{
    public int Value { get; private init; }

    private Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        Value = value;
    }

    public static Quantity Create(int value)
        => new(value);

    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(int value) => Create(value);

    public static Quantity operator +(Quantity left, Quantity right)
        => Create(left.Value + right.Value);

    public static Quantity operator -(Quantity left, Quantity right)
    {
        int result = left.Value - right.Value;
        if (result < 0)
            throw new InvalidOperationException("Quantity cannot be negative");

        return Create(result);
    }
}