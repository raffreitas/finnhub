using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
public record struct Money : IValueObject
{
    public decimal Value { get; private set; }

    private Money(decimal value) => Value = value;

    public static Money Create(decimal value)
        => new(value);
}
