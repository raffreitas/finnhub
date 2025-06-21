namespace FinnHub.Shared.Core;

public sealed record PaginatedParams
{
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
}
