using System.Text;

using FinnHub.PortfolioManagement.Application.Abstractions.Tokens;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Services;
using FinnHub.PortfolioManagement.Infrastructure.Authentication.Settings;
using FinnHub.Shared.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinnHub.PortfolioManagement.Infrastructure.Authentication.Setup;
public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetAndConfigureSettings<AuthenticationSettings>(configuration, AuthenticationSettings.SectionName);

        services.AddHttpContextAccessor();

        services.AddJwtAuthentication(settings);

        services.AddScoped<IUserContext, UserContextService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    public static WebApplication UseAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AuthenticationSettings settings)
    {
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
}
