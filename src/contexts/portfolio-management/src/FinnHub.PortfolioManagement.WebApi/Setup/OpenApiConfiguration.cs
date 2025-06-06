using Scalar.AspNetCore;

namespace FinnHub.PortfolioManagement.WebApi.Setup;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}
