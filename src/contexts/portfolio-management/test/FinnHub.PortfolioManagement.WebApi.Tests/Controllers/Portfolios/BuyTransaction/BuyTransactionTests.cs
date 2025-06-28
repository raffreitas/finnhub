using System.Net;
using System.Text.Json;

using Bogus;

using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.WebApi.Models;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using NSubstitute;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers.Portfolios.BuyTransaction;

[Trait("WebApi", "Portfolio/Buy - Endpoints")]
public class SellTransactionTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    private readonly IMarketDataService _marketDataServiceMock = factory.MarketDataServiceMock;
    private readonly Portfolio _portfolioMock = factory.PortfolioMock;

    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnCreated_WhenRequestIsValid))]
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

        _marketDataServiceMock
            .GetCurrentMarketValueAsync(request.AssetSymbol, Arg.Any<CancellationToken>())
            .Returns(Faker.Finance.Amount(10, 100));

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
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

    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid))]
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
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/buy", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = nameof(RegisterBuyTransaction_ShouldReturnNotFound_WhenPortfolioNotFound))]
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
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/buy", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
