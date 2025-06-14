using System.Net;
using System.Text.Json;

using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.WebApi.Tests.Common;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Authentication;

using Shouldly;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Controllers.Portfolio;
public class PortfolioControllerTests(WebApiFactory factory) : WebApiTestFixture(factory)
{
    [Fact(DisplayName = nameof(CreatePortfolio_ShouldReturnCreated_WhenRequestIsValid))]
    [Trait("WebApi", "Portfolio - Controllers")]
    public async Task CreatePortfolio_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreatePortfolioRequest { Name = Faker.Commerce.ProductName() };
        var token = JwtTokenBuilder.Build();

        // Act
        var response = await PostAsync("/api/v1/portfolios", request, token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("id").GetString().ShouldNotBeNull();
        responseData.RootElement.GetProperty("id").GetString().ShouldNotBe(Guid.Empty.ToString());
    }
}
