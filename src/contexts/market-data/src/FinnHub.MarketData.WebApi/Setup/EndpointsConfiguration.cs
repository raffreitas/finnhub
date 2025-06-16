using System.Reflection;

using FinnHub.MarketData.WebApi.Shared.Endpoints;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinnHub.MarketData.WebApi.Setup;

public static class EndpointsConfiguration
{
    public static IServiceCollection AddEndpointsConfiguration(this IServiceCollection services)
    {
        ServiceDescriptor[] serviceDescriptors = Assembly
            .GetExecutingAssembly()
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder UseEndpointsConfiguration(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null
    )
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
            endpoint.MapEndpoint(builder);

        return app;
    }
}
