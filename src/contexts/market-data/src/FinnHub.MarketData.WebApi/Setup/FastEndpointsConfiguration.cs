using FastEndpoints;
using FastEndpoints.Security;

namespace FinnHub.MarketData.WebApi.Setup;

public static class FastEndpointsConfiguration
{
    public static IServiceCollection AddFastEndpointsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationSettings = AuthenticationConfiguration.GetSettings(services, configuration);
        services
            .AddFastEndpoints()
            .AddAuthenticationJwtBearer(s => s.SigningKey = authenticationSettings.JwtSecret);

        return services;
    }

    public static IApplicationBuilder UseFastEndpointsConfiguration(this IApplicationBuilder app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Versioning.Prefix = "v";
            c.Versioning.PrependToRoute = true;
        });
        return app;
    }
}
