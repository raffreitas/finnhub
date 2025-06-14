using System.Text;

using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);

        services.AddHttpContextAccessor();

        services.AddJwtAuthentication(settings);

        services.AddScoped<IUserContext, UserContextService>();

        return services;
    }

    private static AuthenticationSettings GetSettings(IServiceCollection services, IConfiguration configuration, string section = AuthenticationSettings.SectionName)
    {
        services.AddOptions<AuthenticationSettings>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration.GetSection(section).Get<AuthenticationSettings>()
            ?? throw new ArgumentException($"{nameof(AuthenticationSettings)} should be configured.");

        return settings;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AuthenticationSettings settings)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.JwtSecret)),
            });

        return services;
    }
}
