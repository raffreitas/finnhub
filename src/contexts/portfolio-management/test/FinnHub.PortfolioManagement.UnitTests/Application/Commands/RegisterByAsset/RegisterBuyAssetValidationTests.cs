using FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;
using FinnHub.PortfolioManagement.UnitTests.Application.Commands.Common;

using Shouldly;

namespace FinnHub.PortfolioManagement.UnitTests.Application.Commands.RegisterByAsset;

[Trait("UnitTests", "Application - Portfolio - Validators")]
public sealed class RegisterBuyAssetValidationTests(CommandsBaseFixture fixture) : IClassFixture<CommandsBaseFixture>
{
    [Fact]
    public void Validate_ShouldReturnError_WhenPortfolioIdIsEmpty()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.Empty,
            AssetSymbol = fixture.Faker.Random.String(7),
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = DateTimeOffset.UtcNow
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Portfolio Id' must not be empty.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenAssetSymbolIsEmpty()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = string.Empty,
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = DateTimeOffset.UtcNow
        };
        // Act  
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(2);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Asset Symbol' must not be empty.");
        validationResult.Errors[1].ErrorMessage.ShouldBe("The length of 'Asset Symbol' must be at least 3 characters. You entered 0 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenAssetSymbolIsLessThan3Characters()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = fixture.Faker.Random.String2(2),
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = DateTimeOffset.UtcNow
        };
        // Act  
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("The length of 'Asset Symbol' must be at least 3 characters. You entered 2 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenAssetSymbolIsGreaterThan10Characters()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = fixture.Faker.Random.String2(11),
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = DateTimeOffset.UtcNow
        };
        // Act  
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("The length of 'Asset Symbol' must be 10 characters or fewer. You entered 11 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTransactionDateIsDefault()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = fixture.Faker.Random.String(7),
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = default
        };

        // Act
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Transaction Date' must not be empty.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenQuantityIsZeroOrLess()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = fixture.Faker.Random.String(7),
            PricePerUnit = fixture.Faker.Random.Decimal(1, 100),
            Quantity = 0,
            TransactionDate = DateTimeOffset.UtcNow
        };
        // Act  
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Quantity' must be greater than '0'.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPricePerUnitIsZeroOrLess()
    {
        // Arrange  
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.NewGuid(),
            AssetSymbol = fixture.Faker.Random.String(7),
            PricePerUnit = 0,
            Quantity = fixture.Faker.Random.Int(1, 100),
            TransactionDate = DateTimeOffset.UtcNow
        };
        // Act  
        var validationResult = request.Validate();
        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Price Per Unit' must be greater than '0'.");
    }
}
