using FinnHub.MarketData.WebApi.Setup;

namespace FinnHub.MarketData.WebApi;

public static class StartupHelper
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.AddLoggingConfiguration();

        builder.Services.AddCorsConfiguration();

        builder.Services.AddAuthenticationConfiguration();

        builder.Services.AddHealthCheckConfiguration();

        builder.Services.AddFastEndpointsConfiguration(builder.Configuration);

        builder.Services.AddOpenApiConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseCorsConfiguration();

        app.UseLoggingConfiguration();

        app.UseAuthenticationConfiguration();

        app.UseHealthCheckConfiguration();

        app.UseFastEndpointsConfiguration();

        app.UseOpenApiConfiguration();

        app.UseHttpsRedirection();
    }
}
