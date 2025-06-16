using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

namespace FinnHub.PortfolioManagement.WebApi.Setup;

internal static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddDocumentTransformer<OpenApiDocumentTransformer>();
        });

        return services;
    }

    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options => options
                .WithTitle("FinnHub - Portfolio Management API")
            );
        }

        return app;
    }

    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authSchemes.Any(scheme => scheme.Name == JwtBearerDefaults.AuthenticationScheme))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    [JwtBearerDefaults.AuthenticationScheme] = new()
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                        In = ParameterLocation.Header,
                        BearerFormat = "JSON Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;
            }
        }
    }

    internal sealed class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Info = new()
            {
                Title = "FinnHub - Portfolio Management API",
                Version = "v1",
                Description = "API for managing portfolios, stocks, and trades.",
            };

            return Task.CompletedTask;
        }
    }
}
