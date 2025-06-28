using System.Net;
using System.Text.Json;

using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers.Portfolios.CreatePortfolio;

[Trait("WebApi", "Portfolio/Create - Endpoints")]
public class CreatePortfolioTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    private readonly Portfolio _portfolioMock = factory.PortfolioMock;

    [Fact(DisplayName = nameof(CreatePortfolio_ShouldReturnCreated_WhenRequestIsValid))]
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
    public async Task CreatePortfolio_ShouldReturnConflict_WhenPortfolioNameIsTaken()
    {
        // Arrange
        var request = new CreatePortfolioRequest { Name = _portfolioMock.Name };
        var token = new JwtTokenBuilder().WithUserId(_portfolioMock.UserId).Build();

        // Act
        var response = await PostAsync("/api/v1/portfolios", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
