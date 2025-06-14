using FinnHub.PortfolioManagement.Application.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Logger.Setup;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Setup;
using FinnHub.PortfolioManagement.WebApi.Setup;

namespace FinnHub.PortfolioManagement.WebApi;

public static class StartupHelper
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.AddTelemetryConfiguration();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddOpenApiConfiguration();
        builder.Services.AddPersistenceConfiguration(builder.Configuration);
        // TODO: Add HttpContext in authentication layer
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthenticationConfiguration();
        builder.Services.AddApplicationConfiguration();
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
