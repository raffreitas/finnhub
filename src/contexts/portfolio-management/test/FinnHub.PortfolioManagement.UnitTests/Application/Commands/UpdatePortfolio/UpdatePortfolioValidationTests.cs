using FinnHub.PortfolioManagement.Application.Commands.UpdatePortfolio;
using FinnHub.PortfolioManagement.UnitTests.Application.Commands.Common;

using Shouldly;

namespace FinnHub.PortfolioManagement.UnitTests.Application.Commands.UpdatePortfolio;

[Trait("UnitTests", "Application - Portfolio - Validators")]
public sealed class UpdatePortfolioValidationTests(CommandsBaseFixture fixture) : IClassFixture<CommandsBaseFixture>
{
    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmpty()
    {
        // Arrange  
        var request = new UpdatePortfolioRequest
        {
            Id = Guid.Empty
        };

        // Act  
        var validationResult = request.Validate();

        // Assert
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Id' must not be empty.");
    }

    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenNameIsLessThen3Characters))]
    public void Validate_ShouldReturnError_WhenNameIsLessThen3Characters()
    {
        // Arrange
        var name = fixture.Faker.Random.String2(2);
        var request = new UpdatePortfolioRequest
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid
            .ShouldBeFalse();
        validationResult.Errors
            .ShouldHaveSingleItem();
        validationResult.Errors[0]
            .ErrorMessage
            .ShouldBe($"The length of 'Name' must be at least 3 characters. You entered {name.Length} characters.");
    }

    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenNameIsGreatherThen50Characters))]
    public void Validate_ShouldReturnError_WhenNameIsGreatherThen50Characters()
    {
        // Arrange
        var name = fixture.Faker.Random.String2(51);
        var request = new UpdatePortfolioRequest
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid
            .ShouldBeFalse();
        validationResult.Errors
            .ShouldHaveSingleItem();
        validationResult.Errors[0]
            .ErrorMessage
            .ShouldBe($"The length of 'Name' must be 50 characters or fewer. You entered {name.Length} characters.");
    }

    [Fact(DisplayName = nameof(Validate_ShouldReturnSuccess_WhenNameIsValid))]
    public void Validate_ShouldReturnSuccess_WhenNameIsValid()
    {
        // Arrange
        var name = fixture.Faker.Commerce.ProductName();
        var request = new UpdatePortfolioRequest
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.ShouldBeEmpty();
    }

    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenDescriptionIsGreaterThen500Characters))]
    public void Validate_ShouldReturnError_WhenDescriptionIsGreaterThen500Characters()
    {
        // Arrange
        var description = fixture.Faker.Random.String2(501);
        var request = new UpdatePortfolioRequest
        {
            Id = Guid.NewGuid(),
            Description = description
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid
           .ShouldBeFalse();
        validationResult.Errors
            .ShouldHaveSingleItem();
        validationResult.Errors[0]
            .ErrorMessage
            .ShouldBe($"The length of 'Description' must be 500 characters or fewer. You entered {description.Length} characters.");
    }
}
