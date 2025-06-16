using FinnHub.MarketData.WebApi.Setup;

namespace FinnHub.MarketData.WebApi;

public static class StartupHelper
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.AddLoggingConfiguration();

        builder.Services.AddCorsConfiguration();

        builder.Services.AddAuthenticationConfiguration(builder.Configuration);

        builder.Services.AddHealthCheckConfiguration();

        builder.Services.AddEndpointsConfiguration();

        builder.Services.AddOpenApiConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseCorsConfiguration();

        app.UseLoggingConfiguration();

        app.UseAuthenticationConfiguration();

        app.UseHealthCheckConfiguration();

        app.UseEndpointsConfiguration();

        app.UseOpenApiConfiguration();

        app.UseHttpsRedirection();
    }
}
