using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Portfolio> Portfolios { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
