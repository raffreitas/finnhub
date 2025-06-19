namespace FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Context;
public interface ICorrelationContextAccessor
{
    CorrelationContext? Context { get; set; }
}
