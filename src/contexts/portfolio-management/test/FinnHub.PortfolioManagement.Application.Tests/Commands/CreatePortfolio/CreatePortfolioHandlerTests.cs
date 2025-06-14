using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.Application.Tests.Commands.Common;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;

using NSubstitute;

using Shouldly;

namespace FinnHub.PortfolioManagement.Application.Tests.Commands.CreatePortfolio;

public class CreatePortfolioHandlerTests(CommandsBaseFixture fixture) : IClassFixture<CommandsBaseFixture>
{
    private readonly IPortfolioRepository _portfolioRepository = Substitute.For<IPortfolioRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    [Trait("Application", "CreatePortfolio - Handlers")]
    public async Task Handle_ShouldReturnError_WhenValidationIsFailed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreatePortfolioRequest
        {
            Name = string.Empty,
            Description = fixture.Faker.Lorem.Sentence(50)
        };
        _portfolioRepository
            .NameExistsAsync(Arg.Is(userId), request.Name, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContext.UserId.Returns(userId);
        var handler = new CreatePortfolioHandler(_portfolioRepository, _unitOfWork, _userContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("'Name' must not be empty.");
        result.Errors.ShouldContain("The length of 'Name' must be at least 3 characters. You entered 0 characters.");
    }

    [Fact]
    [Trait("Application", "CreatePortfolio - Handlers")]
    public async Task Handle_ShouldReturnError_WhenNameAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreatePortfolioRequest
        {
            Name = fixture.Faker.Commerce.ProductName(),
            Description = fixture.Faker.Lorem.Sentence(50)
        };
        _portfolioRepository
            .NameExistsAsync(Arg.Is(userId), request.Name, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContext.UserId.Returns(userId);
        var handler = new CreatePortfolioHandler(_portfolioRepository, _unitOfWork, _userContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("A portfolio with this name already exists.");
    }

    [Fact(DisplayName = nameof(Handle_ShouldReturnWithSuccess_WhenAllRulesMatches))]
    [Trait("Application", "CreatePortfolio - Handlers")]
    public async Task Handle_ShouldReturnWithSuccess_WhenAllRulesMatches()
    {
        // Arrange  
        var userId = Guid.NewGuid();
        var request = new CreatePortfolioRequest
        {
            Name = fixture.Faker.Commerce.ProductName(),
            Description = fixture.Faker.Lorem.Sentence(50)
        };
        _portfolioRepository
            .NameExistsAsync(Arg.Is(userId), request.Name, Arg.Any<CancellationToken>())
            .Returns(false);
        _userContext.UserId.Returns(userId);
        var handler = new CreatePortfolioHandler(_portfolioRepository, _unitOfWork, _userContext);

        // Act  
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert  
        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(Guid.Empty);

        await _portfolioRepository
            .Received(1)
            .AddAsync(Arg.Is<Portfolio>(p => p.Name == request.Name && p.Description == request.Description && p.UserId == userId), Arg.Any<CancellationToken>());

        await _unitOfWork
            .Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());
    }
}
