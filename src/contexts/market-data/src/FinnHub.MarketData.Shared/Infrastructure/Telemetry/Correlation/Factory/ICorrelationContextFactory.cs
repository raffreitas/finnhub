using FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Context;

namespace FinnHub.MarketData.Shared.Infrastructure.Telemetry.Correlation.Factory;
public interface ICorrelationContextFactory
{
    public CorrelationContext Create(string correlationId);
    public CorrelationContext Create();
}
