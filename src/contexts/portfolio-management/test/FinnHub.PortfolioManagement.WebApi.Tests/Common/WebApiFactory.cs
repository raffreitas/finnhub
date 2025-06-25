using DotNet.Testcontainers.Builders;

using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Aggregates;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace FinnHub.PortfolioManagement.WebApi.Tests.Common;
public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("portfolio-management")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging();
            });
        });

        base.ConfigureWebHost(builder);
    }

    public Portfolio FakePortfolio { get; private set; } = default!;

    private async Task SeedDatabase(ApplicationDbContext dbContext)
    {
        FakePortfolio = new PortfolioBuilder().Build();
        dbContext.Portfolios.Add(FakePortfolio);
        await dbContext.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await dbContext.Database.MigrateAsync();
            await SeedDatabase(dbContext);
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
