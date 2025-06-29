using FinnHub.MarketData.Processor;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    Startup.ConfigureHost(builder);
    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    return;
}
