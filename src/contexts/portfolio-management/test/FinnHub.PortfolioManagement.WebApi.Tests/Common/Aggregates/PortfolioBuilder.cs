using Bogus;

using FinnHub.PortfolioManagement.Domain.Aggregates;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Common.Aggregates;

internal sealed class PortfolioBuilder
{
    private readonly Faker _faker = new();

    private Guid UserId { get; set; }
    private string Name { get; set; }
    private string? Description { get; set; }

    public PortfolioBuilder()
    {
        UserId = Guid.NewGuid();
        Name = _faker.Commerce.ProductName();
    }

    public PortfolioBuilder WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }

    public PortfolioBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public PortfolioBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public Portfolio Build()
    {
        var portfolio = Portfolio.Create(UserId, Name, Description);
        portfolio.BuyAsset("AAPL", _faker.Random.Int(1, 100), _faker.Random.Decimal(min: 0) * 1000, _faker.Random.Decimal(min: 0) * 1000, _faker.Date.Past());
        portfolio.BuyAsset("MSFT", _faker.Random.Int(1, 100), _faker.Random.Decimal(min: 0) * 1000, _faker.Random.Decimal(min: 0) * 1000, _faker.Date.Past());
        return portfolio;
    }
}
