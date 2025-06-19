using FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Context;

namespace FinnHub.PortfolioManagement.Infrastructure.Telemetry.Correlation.Factory;
public interface ICorrelationContextFactory
{
    public CorrelationContext Create(string correlationId);
    public CorrelationContext Create();
}
