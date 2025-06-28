using DotNet.Testcontainers.Builders;

using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Infrastructure.Caching.Settings;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Settings;
using FinnHub.PortfolioManagement.WebApi.Tests.Common.Aggregates;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
        .WithCleanUp(true)
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .WithPortBinding(6379, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
        .WithCleanUp(true)
        .Build();

    public IMarketDataService MarketDataServiceMock { get; private set; } = Substitute.For<IMarketDataService>();
    public Portfolio PortfolioMock { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting($"{DatabaseSettings.SectionName}:ConnectionString", _dbContainer.GetConnectionString());
        builder.UseSetting($"{CacheSettings.SectionName}:ConnectionString", _redisContainer.GetConnectionString());

        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
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
