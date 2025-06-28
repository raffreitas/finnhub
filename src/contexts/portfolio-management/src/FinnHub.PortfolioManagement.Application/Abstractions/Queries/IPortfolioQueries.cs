using FinnHub.PortfolioManagement.Application.DTOs;
using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Abstractions.Queries;

public interface IPortfolioQueries
{
    Task<PaginatedResult<PortfolioSummaryResponseDto>> GetPortfoliosSummaryAsync(
        Guid userId,
        PaginatedParams paginatedParams,
        CancellationToken cancellationToken = default
    );

    Task<PaginatedResult<PortfolioTransactionsResponseDto>> GetTransactionsAsync(
        Guid userId,
        Guid portfolioId,
        PaginatedParams paginatedParams,
        CancellationToken cancellationToken = default
    );
}
