namespace FinnHub.PortfolioManagement.Domain.SeedWork;

public sealed record PaginatedParams
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; } = 10;
}
