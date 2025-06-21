using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;
using FinnHub.Shared.Kernel;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Position> Positions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<DomainEvent>();

        modelBuilder.ApplyConfiguration(new PortfolioConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new PositionConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
