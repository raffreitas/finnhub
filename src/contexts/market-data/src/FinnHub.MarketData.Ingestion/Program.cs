using FinnHub.MarketData.Ingestion;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    Startup.ConfigureHost(builder);
    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    var logger = factory.CreateLogger<HostApplicationBuilder>();
    logger.LogCritical(ex, "An unhandled exception occurred during application startup.");
}
