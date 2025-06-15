using Microsoft.Extensions.Primitives;

namespace FinnHub.MarketData.WebApi.Shared.Middleware;

internal class LogScopeMiddleware(RequestDelegate next, ILogger<LogScopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestHeaders = context.Request.Headers;
        var correlationId = requestHeaders.TryGetValue("X-Correlation-ID", out StringValues value)
            ? value.ToString()
            : Guid.NewGuid().ToString();

        using (logger.BeginScope("{CorrelationId}", correlationId))
        {
            await next(context);
        }
    }
}
