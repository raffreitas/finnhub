using System.Text;

using FinnHub.MarketData.WebApi.Shared.Infrastructure.Authentication.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FinnHub.MarketData.WebApi.Setup;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = GetSettings(services, configuration);

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.JwtSecret)),
            });
        return services;
    }

    public static WebApplication UseAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        return app;
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
}
