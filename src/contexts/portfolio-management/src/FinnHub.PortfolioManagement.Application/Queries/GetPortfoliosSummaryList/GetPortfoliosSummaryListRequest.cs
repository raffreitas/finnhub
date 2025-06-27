using FinnHub.Shared.Core;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfoliosSummaryList;

public sealed record GetPortfoliosSummaryListRequest : IRequest<Result<GetPortfoliosSummaryListResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
