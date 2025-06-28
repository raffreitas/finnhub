using System.Net.Http.Headers;
using System.Net.Http.Json;

using Bogus;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Common;

public abstract class WebApiTestFixture(WebApiFactory factory) : IClassFixture<WebApiFactory>
{
    protected static Faker Faker => new();

    private readonly HttpClient _httpClient = factory.CreateClient();

    protected async Task<HttpResponseMessage> PostAsync<T>(string method, T request, string token = "")
    {
        AuthenticateRequest(token);

        return await _httpClient.PostAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string method, T request, string token = "")
    {
        AuthenticateRequest(token);

        return await _httpClient.PutAsJsonAsync(method, request);
    }

    private void AuthenticateRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
