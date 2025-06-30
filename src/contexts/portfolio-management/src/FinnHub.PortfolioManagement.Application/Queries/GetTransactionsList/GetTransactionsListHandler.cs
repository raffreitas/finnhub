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
        var paginatedParams = new PaginatedParams(request.PageNumber, request.PageSize);

        var result = await portfolioQueries.GetTransactionsAsync(
            userContext.UserId,
            request.PortfolioId,
            paginatedParams,
            cancellationToken);

        return new GetTransactionsListResponse(result);
    }
}
