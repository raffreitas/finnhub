using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.Enums;
using FinnHub.PortfolioManagement.Domain.Aggregates.ValueObjects;

using Shouldly;

namespace FinnHub.PortfolioManagement.Domain.UnitTests.Aggregates;

public class PortfolioTests(PortifolioTestsFixture fixture) : IClassFixture<PortifolioTestsFixture>
{
    [Fact(DisplayName = nameof(CreateBuyTransaction_ShouldCreateTransaction_WhenValidParameters))]
    [Trait("Domain", "Portfolio - Aggregates")]
    public void CreateBuyTransaction_ShouldCreateTransaction_WhenValidParameters()
    {
        // Arrange
        var portfolioId = fixture.Faker.Random.Guid();
        var assetSymbol = AssetSymbol.Create(fixture.Faker.Commerce.ProductName()[..10]);
        var quantity = Quantity.Create(fixture.Faker.Random.Int(1, 100));
        var price = Money.Create(fixture.Faker.Random.Decimal(1.0m, 1000.0m));
        var currentMarketValue = Money.Create(fixture.Faker.Random.Decimal(1.0m, 1000.0m));
        var transactionDate = DateTimeOffset.UtcNow;
        // Act

        var transaction = Transaction.CreateBuyTransaction(portfolioId, assetSymbol, quantity, price, currentMarketValue, transactionDate);

        // Assert
        portfolioId.ShouldBe(transaction.PortfolioId);
        assetSymbol.ShouldBe(transaction.AssetSymbol);
        transaction.Type.ShouldBe(TransactionType.Buy);
        quantity.ShouldBe(transaction.Quantity);
        price.ShouldBe(transaction.Price);
        transaction.TransactionDate.ShouldBe(transactionDate);
        transaction.IsSettled.ShouldBeFalse();
    }
}
