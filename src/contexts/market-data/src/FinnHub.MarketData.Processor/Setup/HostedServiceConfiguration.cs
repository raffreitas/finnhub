using FinnHub.MarketData.Processor.Workers;

namespace FinnHub.MarketData.Processor.Setup;
internal static class HostedServiceConfiguration
{
    public static IServiceCollection AddHostedServicesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<DataIngestionProcessorService>();
        return services;
    }
}
