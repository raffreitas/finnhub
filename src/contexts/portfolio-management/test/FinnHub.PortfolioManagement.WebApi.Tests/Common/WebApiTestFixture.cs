using System.Net.Http.Headers;
using System.Net.Http.Json;

using Bogus;

using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Common;

public abstract class WebApiTestFixture : IClassFixture<WebApiFactory>
{
    protected static Faker Faker => new();

    private readonly HttpClient _httpClient;

    protected WebApiTestFixture(WebApiFactory factory)
    {
        _httpClient = factory.CreateClient();

        var serviceScope = factory.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
            dbContext.Database.Migrate();
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string method, T request, string token = "")
    {
        AuthenticateRequest(token);

        return await _httpClient.PostAsJsonAsync(method, request);
    }


    private void AuthenticateRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
