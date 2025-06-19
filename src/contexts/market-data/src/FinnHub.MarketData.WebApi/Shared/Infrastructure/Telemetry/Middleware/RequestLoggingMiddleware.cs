using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FinnHub.MarketData.WebApi.Shared.Infrastructure.Telemetry.Middleware;
internal sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        var logState = new Dictionary<string, object>
        {
            ["RequestMethod"] = context.Request.Method,
            ["RequestPath"] = context.Request.Path,
            ["RequestQueryString"] = context.Request.QueryString.ToString()
        };

        using (logger.BeginScope(logState))
        {
            await next(context);
        }

        sw.Stop();

        logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds
        );
    }
}
