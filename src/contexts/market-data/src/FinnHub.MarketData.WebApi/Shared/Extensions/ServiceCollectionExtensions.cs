namespace FinnHub.MarketData.WebApi.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static T GetAndConfigureSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName
    ) where T : class
    {
        services.AddOptions<T>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration.GetSection(sectionName).Get<T>()
            ?? throw new ArgumentException($"{nameof(T)} should be configured.");

        return settings;
    }
}
