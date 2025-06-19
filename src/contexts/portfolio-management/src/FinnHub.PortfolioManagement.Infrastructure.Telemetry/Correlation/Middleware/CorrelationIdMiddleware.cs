using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Factory;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Middleware;
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
        if (!context.Response.Headers.ContainsKey("X-Correlation-ID"))
            context.Response.Headers.Append("X-Correlation-ID", correlationId);
        else
            context.Response.Headers["X-Correlation-ID"] = correlationId;
    }

    private static void IncludeCorrelationIdResponseHeader(HttpResponse response, string correlationId)
    {
        if (!response.Headers.ContainsKey("X-Correlation-ID"))
            response.Headers.Append("X-Correlation-ID", correlationId);
        else
            response.Headers["X-Correlation-ID"] = correlationId;
    }

    private static void IncludeInResponse(HttpContext context, string correlationId)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            return Task.CompletedTask;
        });
    }
}
