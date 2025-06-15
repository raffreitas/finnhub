using FinnHub.MarketData.WebApi.Shared.Settings;

namespace FinnHub.MarketData.WebApi.Setup;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization();
        return services;
    }

    public static WebApplication UseAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthorization();
        return app;
    }

    public static AuthenticationSettings GetSettings(IServiceCollection services, IConfiguration configuration, string section = AuthenticationSettings.SectionName)
    {
        services.AddOptions<AuthenticationSettings>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration.GetSection(section).Get<AuthenticationSettings>()
            ?? throw new ArgumentException($"{nameof(AuthenticationSettings)} should be configured.");

        return settings;
    }
}
