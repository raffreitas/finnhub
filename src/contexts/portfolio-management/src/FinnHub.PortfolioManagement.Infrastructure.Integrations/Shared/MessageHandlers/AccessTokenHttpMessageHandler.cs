using System.Net.Http.Headers;

using FinnHub.PortfolioManagement.Application.Abstractions.Tokens;

namespace FinnHub.PortfolioManagement.Infrastructure.Integrations.Shared.MessageHandlers;
internal sealed class AccessTokenHttpMessageHandler(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var token = await tokenService.GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
