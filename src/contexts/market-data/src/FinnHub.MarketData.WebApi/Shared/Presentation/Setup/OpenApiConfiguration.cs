using Scalar.AspNetCore;

namespace FinnHub.MarketData.WebApi.Shared.Presentation.Setup;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        return services.AddOpenApi();
    }

    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options => options
                .WithTitle("FinnHub - Market Data API")
            );
        }

        return app;
    }
}
