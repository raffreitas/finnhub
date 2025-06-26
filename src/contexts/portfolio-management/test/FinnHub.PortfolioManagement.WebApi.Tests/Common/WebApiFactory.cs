using DotNet.Testcontainers.Builders;

using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Aggregates;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NSubstitute;

using Testcontainers.PostgreSql;
using Testcontainers.Redis;

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

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .WithPortBinding(6379, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
        .Build();

    public IMarketDataService MarketDataServiceMock { get; private set; } = Substitute.For<IMarketDataService>();
    public Portfolio PortfolioMock { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureTestServices(services =>
            {
                #region Database Configuration
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
                #endregion

                #region Cache Configuration
                services.RemoveAll<HybridCache>();
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisContainer.GetConnectionString();
                    options.InstanceName = "portfolio-management-test";
                });
                services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    LocalCacheExpiration = TimeSpan.FromMinutes(5),
                    Expiration = TimeSpan.FromMinutes(5)
                });
                #endregion

                services.RemoveAll<IMarketDataService>();
                services.AddScoped(x => MarketDataServiceMock);
            });

        base.ConfigureWebHost(builder);
    }
    private async Task SeedDatabaseAsync(ApplicationDbContext dbContext)
    {
        PortfolioMock = new PortfolioBuilder().Build();
        await dbContext.Portfolios.AddAsync(PortfolioMock);
        await dbContext.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await dbContext.Database.MigrateAsync();
            await SeedDatabaseAsync(dbContext);
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
