using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace FinnHub.PortfolioManagement.Application.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        services
            .AddMediatR(cfg => cfg
                .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
        return services;
    }
}
