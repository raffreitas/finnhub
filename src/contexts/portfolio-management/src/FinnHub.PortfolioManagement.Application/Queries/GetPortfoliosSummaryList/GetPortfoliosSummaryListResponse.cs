using FinnHub.PortfolioManagement.Application.DTOs;
using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfoliosSummaryList;

public sealed record GetPortfoliosSummaryListResponse : PaginatedResult<PortfolioSummaryResponseDto>
{
    public GetPortfoliosSummaryListResponse(PaginatedResult<PortfolioSummaryResponseDto> original) : base(original)
    {
    }

    public GetPortfoliosSummaryListResponse(
        int pageNumber,
        int pageSize,
        int totalCount,
        IEnumerable<PortfolioSummaryResponseDto> items
    ) : base(pageNumber, pageSize, totalCount, items)
    {
    }
};

