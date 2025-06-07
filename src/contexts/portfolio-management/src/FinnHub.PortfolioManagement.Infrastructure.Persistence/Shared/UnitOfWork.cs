using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Shared;
internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
