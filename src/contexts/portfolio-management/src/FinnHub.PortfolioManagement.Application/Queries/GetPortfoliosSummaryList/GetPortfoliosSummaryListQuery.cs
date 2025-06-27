using FinnHub.PortfolioManagement.Application.Abstractions.Queries;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.Shared.Core;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfoliosSummaryList;

internal sealed class GetPortfoliosSummaryListQuery(
    IPortfolioQueries portfolioQueries,
    IUserContext userContext
) : IRequestHandler<GetPortfoliosSummaryListRequest, Result<GetPortfoliosSummaryListResponse>>
{
    public async Task<Result<GetPortfoliosSummaryListResponse>> Handle(GetPortfoliosSummaryListRequest request, CancellationToken cancellationToken)
    {


        var paginatedParams = new PaginatedParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        var response = await portfolioQueries.GetPortfoliosSummaryAsync(userContext.UserId, paginatedParams, cancellationToken);
        return new GetPortfoliosSummaryListResponse(response);
    }
}
