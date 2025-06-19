using FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Factory;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Correlation.Middleware;
internal sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ICorrelationContextFactory correlationContextFactory,
    ILogger<CorrelationIdMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);

        correlationContextFactory.Create(correlationId);

        IncludeCorrelationIdRequestHeader(context, correlationId);
        IncludeCorrelationIdResponseHeader(context.Response, correlationId);
        IncludeInResponse(context, correlationId);

        using (logger.BeginScope("{CorrelationId}", correlationId))
        {
            await next(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        if (
            context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationInHeader) &&
            correlationInHeader.FirstOrDefault() is { Length: > 0 } correlationIdValue
        )
        {
            return correlationIdValue;
        }

        return Guid.NewGuid().ToString("N");
    }

    private static void IncludeCorrelationIdRequestHeader(HttpContext context, string correlationId)
    {
        SetCorrelationIdHeader(context.Response.Headers, correlationId);
    }

    private static void IncludeCorrelationIdResponseHeader(HttpResponse response, string correlationId)
    {
        SetCorrelationIdHeader(response.Headers, correlationId);
    }

    private static void IncludeInResponse(HttpContext context, string correlationId)
    {
        context.Response.OnStarting(() =>
        {
            SetCorrelationIdHeader(context.Response.Headers, correlationId);
            return Task.CompletedTask;
        });
    }
    private static void SetCorrelationIdHeader(IHeaderDictionary headers, string correlationId)
    {
        if (!headers.ContainsKey("X-Correlation-ID"))
            headers.Append("X-Correlation-ID", correlationId);
        else
            headers["X-Correlation-ID"] = correlationId;
    }
}
