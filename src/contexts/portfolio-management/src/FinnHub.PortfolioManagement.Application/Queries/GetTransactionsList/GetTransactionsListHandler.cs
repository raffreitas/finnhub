using FinnHub.PortfolioManagement.Application.Abstractions.Queries;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.Shared.Core;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Queries.GetTransactionsList;
internal sealed class GetTransactionsListHandler(
    IPortfolioQueries portfolioQueries,
    IUserContext userContext
) : IRequestHandler<GetTransactionsListRequest, Result<GetTransactionsListResponse>>
{
    public async Task<Result<GetTransactionsListResponse>> Handle(GetTransactionsListRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement PaginatedParams
        var paginatedParams = new PaginatedParams
        {
            PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
            PageSize = request.PageSize == 0 ? 10 : request.PageSize
        };

        var result = await portfolioQueries.GetTransactionsAsync(
            userContext.UserId,
            request.PortfolioId,
            paginatedParams,
            cancellationToken);

        return new GetTransactionsListResponse(result);
    }
}
