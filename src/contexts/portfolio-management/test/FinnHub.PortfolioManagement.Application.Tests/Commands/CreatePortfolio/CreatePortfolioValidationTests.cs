using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.Application.Tests.Commands.Common;

using Shouldly;

namespace FinnHub.PortfolioManagement.Application.Tests.Commands.CreatePortfolio;
public class CreatePortfolioValidationTests(CommandsBaseFixture fixture)
    : IClassFixture<CommandsBaseFixture>
{
    [Trait("Application", "CreatePortfolio - Validators")]
    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenNameIsEmpty))]
    public void Validate_ShouldReturnError_WhenNameIsEmpty()
    {
        // Arrange  
        var request = new CreatePortfolioRequest
        {
            Name = string.Empty
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(2);
        validationResult.Errors[0].ErrorMessage.ShouldBe("'Name' must not be empty.");
        validationResult.Errors[1].ErrorMessage.ShouldBe("The length of 'Name' must be at least 3 characters. You entered 0 characters.");
    }

    [Trait("Application", "CreatePortfolio - Validators")]
    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenNameIsLessThen3Characters))]
    public void Validate_ShouldReturnError_WhenNameIsLessThen3Characters()
    {
        // Arrange
        var name = fixture.Faker.Random.String2(2);
        var request = new CreatePortfolioRequest
        {
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

    [Trait("Application", "CreatePortfolio - Validators")]
    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenNameIsGreatherThen50Characters))]
    public void Validate_ShouldReturnError_WhenNameIsGreatherThen50Characters()
    {
        // Arrange
        var name = fixture.Faker.Random.String2(51);
        var request = new CreatePortfolioRequest
        {
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

    [Trait("Application", "CreatePortfolio - Validators")]
    [Fact(DisplayName = nameof(Validate_ShouldReturnSuccess_WhenNameIsValid))]
    public void Validate_ShouldReturnSuccess_WhenNameIsValid()
    {
        // Arrange
        var name = fixture.Faker.Commerce.ProductName();
        var request = new CreatePortfolioRequest
        {
            Name = name
        };

        // Act  
        var validationResult = request.Validate();

        // Assert  
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.ShouldBeEmpty();
    }

    [Trait("Application", "CreatePortfolio - Validators")]
    [Fact(DisplayName = nameof(Validate_ShouldReturnError_WhenDescriptionIsGreaterThen500Characters))]
    public void Validate_ShouldReturnError_WhenDescriptionIsGreaterThen500Characters()
    {
        // Arrange
        var description = fixture.Faker.Random.String2(501);
        var request = new CreatePortfolioRequest
        {
            Name = fixture.Faker.Commerce.ProductName(),
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
