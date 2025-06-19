using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Context;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.HttpClient;
internal sealed class CorrelationIdDelegateHandler(
    ICorrelationContextAccessor correlationContextAccessor
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (
            correlationContextAccessor.Context is { } context && 
            !request.Headers.Contains("X-Correlation-ID")
        )
        {
            request.Headers.Add("X-Correlation-ID", context.CorrelationId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
