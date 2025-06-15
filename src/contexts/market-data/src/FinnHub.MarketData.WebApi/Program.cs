using FinnHub.MarketData.WebApi;

try
{
    var builder = WebApplication.CreateBuilder(args);
    StartupHelper.ConfigureBuilder(builder);
    var app = builder.Build();
    StartupHelper.ConfigureApp(app);
    await app.RunAsync();
}
catch (Exception ex)
{
    using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    var logger = factory.CreateLogger<WebApplication>();
    logger.LogCritical(ex, "An unhandled exception occurred during application startup.");
}
