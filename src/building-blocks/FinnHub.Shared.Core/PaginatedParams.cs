namespace FinnHub.Shared.Core;

public sealed record PaginatedParams
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public PaginatedParams(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
