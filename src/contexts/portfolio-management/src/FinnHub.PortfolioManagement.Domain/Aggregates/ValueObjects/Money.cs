using FinnHub.PortfolioManagement.Domain.Aggregates.Enums;
using FinnHub.PortfolioManagement.Domain.SeedWork;

namespace FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;
public record Money : IValueObject
{
    public decimal Value { get; private init; }
    public Currency Currency { get; private init; }

    private Money(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }

    public static Money Create(decimal value, Currency currency = Currency.USD)
        => new(value, currency);
}
