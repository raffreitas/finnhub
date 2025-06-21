using FinnHub.PortfolioManagement.WebApi.Infrastructure;

namespace FinnHub.PortfolioManagement.WebApi.Setup;

public static class PresentationConfiguration
{
    public static IServiceCollection AddPresentationConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static WebApplication UsePresentationConfiguration(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }
}
