namespace FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;

public interface IPortfolioRepository
{
    Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(Guid userId, string portfolioName, CancellationToken cancellationToken = default);
}
