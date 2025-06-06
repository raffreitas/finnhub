using FinnHub.PortfolioManagement.Infrastructure.Logger.Setup;
using FinnHub.PortfolioManagement.WebApi.Setup;

namespace FinnHub.PortfolioManagement.WebApi;

public static class StartupHelper
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.AddTelemetryConfiguration();

        builder.Services.AddControllers();
        builder.Services.AddOpenApiConfiguration();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseTelemetryConfiguration();

        app.UseOpenApiConfiguration();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
