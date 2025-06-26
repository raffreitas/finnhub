using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Repositories;
internal sealed class PortfolioRepository(ApplicationDbContext dbContext) : IPortfolioRepository
{
    public async Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
        => await dbContext.Portfolios.AddAsync(portfolio, cancellationToken);

    public Task<Portfolio?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        => dbContext.Portfolios
            .Include(x => x.Transactions)
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == id, cancellationToken);

    public async Task<bool> NameExistsAsync(Guid userId, string portfolioName, CancellationToken cancellationToken = default)
        => await dbContext.Portfolios
            .AnyAsync(p => p.UserId == userId && p.Name == portfolioName, cancellationToken);
}
