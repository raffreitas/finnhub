using FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.Shared.Core;

using NSubstitute;

using Shouldly;

namespace FinnHub.PortfolioManagement.UnitTests.Application.Commands.RegisterByAsset;

[Trait("UnitTests", "Application - Portfolio - Commands")]
public sealed class RegisterBuyAssetHandlerTests(RegisterBuyAssetHandlerTestsFixture fixture) : IClassFixture<RegisterBuyAssetHandlerTestsFixture>
{
    private readonly RegisterBuyAssetHandler _handler = new RegisterBuyAssetHandler(
        fixture.PortfolioRepository,
        fixture.UnitOfWork,
        fixture.UserContext,
        fixture.MarketDataService
    );

    [Fact]
    public async Task Handle_ShouldReturnError_WhenValidationIsFailed()
    {
        // Arrange
        var request = new RegisterBuyAssetRequest
        {
            PortfolioId = Guid.Empty,
            AssetSymbol = string.Empty,
            PricePerUnit = 0,
            Quantity = 0,
            TransactionDate = DateTimeOffset.MinValue
        };
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenPortfolioNotFound()
    {
        // Arrange
        var request = fixture.GetValidRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PortfolioErrors.PortfolioNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMarketDataServiceReturnsError()
    {
        // Arrange
        var expectedError = Error.Problem("X", "Y");
        var request = fixture.GetValidRequest();
        fixture.PortfolioRepository
            .GetByIdAsync(Arg.Any<Guid>(), request.PortfolioId, Arg.Any<CancellationToken>())
            .Returns(fixture.Portfolio);
        fixture.MarketDataService
            .GetCurrentMarketValueAsync(request.AssetSymbol, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<decimal>(expectedError));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var request = fixture.GetValidRequest();
        fixture.PortfolioRepository
            .GetByIdAsync(Arg.Any<Guid>(), request.PortfolioId, Arg.Any<CancellationToken>())
            .Returns(fixture.Portfolio);
        fixture.MarketDataService
            .GetCurrentMarketValueAsync(request.AssetSymbol, Arg.Any<CancellationToken>())
            .Returns(100m);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.TransactionId.ShouldNotBe(Guid.Empty);
    }
}
