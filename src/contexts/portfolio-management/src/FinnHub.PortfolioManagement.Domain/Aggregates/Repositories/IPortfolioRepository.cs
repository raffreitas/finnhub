namespace FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;

public interface IPortfolioRepository
{
    Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
    Task<Portfolio?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(Guid userId, string portfolioName, CancellationToken cancellationToken = default);
}
