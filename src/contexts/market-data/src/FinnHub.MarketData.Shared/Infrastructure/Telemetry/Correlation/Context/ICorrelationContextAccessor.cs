namespace FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Context;
public interface ICorrelationContextAccessor
{
    CorrelationContext? Context { get; set; }
}
