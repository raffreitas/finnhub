using FinnHub.PortfolioManagement.Worker.OutboxPublisher.Workers;

namespace FinnHub.PortfolioManagement.Worker.OutboxPublisher.Setup;
internal static class HostedServiceConfiguration
{
    public static IServiceCollection AddHostedServicesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<OutboxPublisherWorker>();
        return services;
    }
}
