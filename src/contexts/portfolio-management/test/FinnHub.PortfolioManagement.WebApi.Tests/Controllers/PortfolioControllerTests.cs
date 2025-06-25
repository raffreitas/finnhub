using System.Net;
using System.Text.Json;

using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.WebApi.Models;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers;
public class PortfolioControllerTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    private readonly Portfolio _fakePortfolio = factory.FakePortfolio;

    #region CreatePortfolio
    [Fact(DisplayName = nameof(CreatePortfolio_ShouldReturnCreated_WhenRequestIsValid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task CreatePortfolio_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreatePortfolioRequest { Name = Faker.Commerce.ProductName() };
        var token = new JwtTokenBuilder().Build();

        // Act
        var response = await PostAsync("/api/v1/portfolios", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("id").GetString().ShouldNotBeNull();
        responseData.RootElement.GetProperty("id").GetString().ShouldNotBe(Guid.Empty.ToString());
    }


    [Fact(DisplayName = nameof(CreatePortfolio_ShouldReturnConflict_WhenPortfolioNameIsTaken))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task CreatePortfolio_ShouldReturnConflict_WhenPortfolioNameIsTaken()
    {
        // Arrange
        var request = new CreatePortfolioRequest { Name = _fakePortfolio.Name };
        var token = new JwtTokenBuilder().WithUserId(_fakePortfolio.UserId).Build();

        // Act
        var response = await PostAsync("/api/v1/portfolios", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }


    #endregion

    #region BuyTransaction
    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnCreated_WhenRequestIsValid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterBuyTransaction_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = Faker.Finance.Account(),
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = Faker.Random.Int(1, 100),
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = _fakePortfolio.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/buy", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("transactionId").GetString().ShouldNotBeNull();
        responseData.RootElement.GetProperty("transactionId").GetString().ShouldNotBe(Guid.Empty.ToString());
    }

    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnNotFound_WhenPortfolioNotFound))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterBuyTransaction_ShouldReturnNotFound_WhenPortfolioNotFound()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = Faker.Finance.Account(),
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = Faker.Random.Int(1, 100),
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = Guid.NewGuid();
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/buy", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterBuyTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = Faker.Finance.Account(),
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = Faker.Random.Int(-100, 0),
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = Guid.NewGuid();
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/buy", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    #endregion

    #region SellTransaction
    [Fact(DisplayName = nameof(RegisterSellTransaction_ShouldReturnCreated_WhenRequestIsValid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterSellTransaction_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = _fakePortfolio.Positions[0].AssetSymbol.Value,
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = _fakePortfolio.Positions[0].Quantity - 1,
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = _fakePortfolio.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/sell", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("transactionId").GetString().ShouldNotBeNull();
        responseData.RootElement.GetProperty("transactionId").GetString().ShouldNotBe(Guid.Empty.ToString());
    }

    [Fact(DisplayName = nameof(RegisterSellTransaction_ShouldReturnNotFound_WhenPortfolioNotFound))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterSellTransaction_ShouldReturnNotFound_WhenPortfolioNotFound()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = Faker.Finance.Account(),
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = Faker.Random.Int(1, 100),
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = Guid.NewGuid();
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/sell", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = nameof(RegisterSellTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task RegisterSellTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = Faker.Finance.Account(),
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = Faker.Random.Int(-100, 0),
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = Guid.NewGuid();
        var token = new JwtTokenBuilder()
            .WithUserId(_fakePortfolio.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/sell", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    #endregion
}
