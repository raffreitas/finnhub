using System.Net;
using System.Text.Json;

using Bogus;

using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.WebApi.Models;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers.Portfolios.SellTransaction;

[Trait("WebApi", "Portfolio/Sell - Endpoints")]
public class SellTransactionTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    private readonly Portfolio _portfolioMock = factory.PortfolioMock;

    [Fact(DisplayName = nameof(RegisterSellTransaction_ShouldReturnCreated_WhenRequestIsValid))]
    public async Task RegisterSellTransaction_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterTransactionModel
        {
            AssetSymbol = _portfolioMock.Positions[0].AssetSymbol.Value,
            PricePerUnit = Faker.Finance.Amount(),
            Quantity = _portfolioMock.Positions[0].Quantity - 1,
            TransactionDate = Faker.Date.Recent()
        };

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
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
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/sell", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = nameof(RegisterSellTransaction_ShouldReturnBadRequest_WhenRequestIsInvalid))]
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
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PostAsync($"/api/v1/portfolios/{portfolioId}/transactions/sell", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
