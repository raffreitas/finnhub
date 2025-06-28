using FinnHub.Shared.Core;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Queries.GetTransactionsList;

public sealed record GetTransactionsListRequest : IRequest<Result<GetTransactionsListResponse>>
{
    public Guid PortfolioId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
