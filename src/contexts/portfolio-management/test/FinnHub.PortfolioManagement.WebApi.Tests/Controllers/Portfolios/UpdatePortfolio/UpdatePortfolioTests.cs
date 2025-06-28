using System.Net;

using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.WebApi.Models;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers.Portfolios.UpdatePortfolio;

[Trait("WebApi", "Portfolio/Update - Endpoints")]
public class UpdatePortfolioTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    private readonly Portfolio _portfolioMock = factory.PortfolioMock;

    [Fact(DisplayName = nameof(UpdatePortfolio_ShouldReturnNoContent_WhenRequestIsValid))]
    public async Task UpdatePortfolio_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdatePortfolioModel
        {
            Name = Faker.Commerce.ProductName(),
            Description = Faker.Lorem.Sentence()
        };

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PutAsync($"/api/v1/portfolios/{portfolioId}", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = nameof(UpdatePortfolio_ShouldReturnNotFound_WhenPortfolioNotFound))]
    public async Task UpdatePortfolio_ShouldReturnNotFound_WhenPortfolioNotFound()
    {
        // Arrange
        var request = new UpdatePortfolioModel
        {
            Name = Faker.Commerce.ProductName(),
            Description = Faker.Lorem.Sentence()
        };

        var portfolioId = Guid.NewGuid();
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PutAsync($"/api/v1/portfolios/{portfolioId}", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = nameof(UpdatePortfolio_ShouldReturnBadRequest_WhenRequestIsInvalid))]
    public async Task UpdatePortfolio_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new UpdatePortfolioModel
        {
            Name = "AB",
            Description = new string('A', 501)
        };

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PutAsync($"/api/v1/portfolios/{portfolioId}", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = nameof(UpdatePortfolio_ShouldReturnNoContent_WhenOnlyNameIsProvided))]
    public async Task UpdatePortfolio_ShouldReturnNoContent_WhenOnlyNameIsProvided()
    {
        // Arrange
        var request = new UpdatePortfolioModel
        {
            Name = Faker.Commerce.ProductName()
        };

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PutAsync($"/api/v1/portfolios/{portfolioId}", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = nameof(UpdatePortfolio_ShouldReturnNoContent_WhenOnlyDescriptionIsProvided))]
    public async Task UpdatePortfolio_ShouldReturnNoContent_WhenOnlyDescriptionIsProvided()
    {
        // Arrange
        var request = new UpdatePortfolioModel
        {
            Description = Faker.Lorem.Sentence()
        };

        var portfolioId = _portfolioMock.Id;
        var token = new JwtTokenBuilder()
            .WithUserId(_portfolioMock.UserId)
            .Build();

        // Act
        var response = await PutAsync($"/api/v1/portfolios/{portfolioId}", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
