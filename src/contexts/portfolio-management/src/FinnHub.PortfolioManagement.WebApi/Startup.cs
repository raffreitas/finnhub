using FinnHub.PortfolioManagement.Application.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Setup;
using FinnHub.PortfolioManagement.WebApi.Setup;

namespace FinnHub.PortfolioManagement.WebApi;

public static class StartupHelper
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.AddTelemetryConfiguration();

        builder.Services.AddControllers();

        builder.Services.AddCorsConfiguration();

        builder.Services.AddOpenApiConfiguration();
        builder.Services.AddPersistenceConfiguration(builder.Configuration);
        builder.Services.AddAuthenticationConfiguration(builder.Configuration);
        builder.Services.AddApplicationConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseTelemetryConfiguration();

        app.UseOpenApiConfiguration();

        app.UseCorsConfiguration();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
    }
}
