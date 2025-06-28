
using FinnHub.PortfolioManagement.Application.DTOs;
using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Queries.GetTransactionsList;

public sealed record GetTransactionsListResponse : PaginatedResult<PortfolioTransactionsResponseDto>
{
    public GetTransactionsListResponse(
        PaginatedResult<PortfolioTransactionsResponseDto> original
    ) : base(original)
    {
    }

    public GetTransactionsListResponse(
        int pageNumber,
        int pageSize,
        int totalCount,
        IEnumerable<PortfolioTransactionsResponseDto> items
    ) : base(pageNumber, pageSize, totalCount, items)
    {
    }
}