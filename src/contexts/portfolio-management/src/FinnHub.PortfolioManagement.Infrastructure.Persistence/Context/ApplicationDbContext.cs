using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
}
