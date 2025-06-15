namespace FinnHub.PortfolioManagement.Domain.SeedWork;
public record PaginatedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public IEnumerable<T> Items { get; init; } = [];

    public PaginatedResult(int pageNumber, int pageSize, int totalCount, IEnumerable<T> items)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new InvalidOperationException("Page number and page size must be greater than or equal to 1.");

        Page = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }
}
