using Dapper;

using FinnHub.PortfolioManagement.Application.Abstractions.Queries;
using FinnHub.PortfolioManagement.Application.DTOs;
using FinnHub.PortfolioManagement.Infrastructure.Persistence.Context;
using FinnHub.Shared.Core;

using Microsoft.EntityFrameworkCore;

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Queries;

internal class PortfolioQueries(ApplicationDbContext dbContext) : IPortfolioQueries
{
    public async Task<PaginatedResult<PortfolioSummaryResponseDto>> GetPortfoliosSummaryAsync(Guid userId, PaginatedParams paginatedParams, CancellationToken cancellationToken = default)
    {
        var offset = (paginatedParams.PageNumber - 1) * paginatedParams.PageSize;

        var total = await dbContext.Portfolios
            .CountAsync(p => p.UserId == userId, cancellationToken);

        const string sql =
            """
            SELECT
                p.id AS Id,
                p.name AS Name,
                p.description AS Description,
                p.creation_date AS CreationDate,
                COALESCE(SUM(pp.current_market_price_value), 0) AS TotalValue,
                COALESCE(SUM(pp.current_market_price_value - pp.average_cost_value), 0) AS TotalProfit
            FROM portfolios p
            LEFT JOIN positions pp ON pp.portfolio_id = p.id
            WHERE p.user_id = @UserId
            GROUP BY p.id, p.name, p.description, p.creation_date
            ORDER BY p.creation_date DESC
            LIMIT @PageSize OFFSET @Offset;
            """;

        using var connection = dbContext.Database.GetDbConnection();
        var result = await connection.QueryAsync<PortfolioSummaryResponseDto>(sql, new
        {
            UserId = userId,
            Offset = offset,
            paginatedParams.PageSize
        });

        return new PaginatedResult<PortfolioSummaryResponseDto>(
            paginatedParams.PageNumber,
            paginatedParams.PageSize,
            total,
            result
        );
    }
}
