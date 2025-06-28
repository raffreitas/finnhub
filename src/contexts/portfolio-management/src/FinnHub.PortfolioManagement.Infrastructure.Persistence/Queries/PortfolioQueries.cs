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

    public async Task<PaginatedResult<PortfolioTransactionsResponseDto>> GetTransactionsAsync(Guid userId, Guid portfolioId, PaginatedParams paginatedParams, CancellationToken cancellationToken = default)
    {
        var offset = (paginatedParams.PageNumber - 1) * paginatedParams.PageSize;

        var portfolioExists = await dbContext.Portfolios
            .AnyAsync(p => p.Id == portfolioId && p.UserId == userId, cancellationToken);

        if (!portfolioExists)
            return new PaginatedResult<PortfolioTransactionsResponseDto>(
                paginatedParams.PageNumber,
                paginatedParams.PageSize,
                0,
                []
            );

        const string countSql =
        """
        SELECT COUNT(*) 
        FROM transactions t
        INNER JOIN portfolios p ON p.id = t.portfolio_id
        WHERE t.portfolio_id = @PortfolioId AND p.user_id = @UserId
        """;

        const string transactionsSql =
        """
        SELECT
            t.id AS Id,
            t.portfolio_id AS PortfolioId,
            t.asset_symbol AS AssetSymbol,
            t.type AS Type,
            t.quantity AS Quantity,
            t.price_value AS Price,
            (t.price_value * t.quantity) AS TotalAmount,
            t.current_market_value_value AS CurrentMarketValue,
            t.transaction_date AS TransactionDate,
            t.is_settled AS IsSettled,
            CASE 
                WHEN t.type = 'Buy' AND t.current_market_value_value IS NOT NULL 
                THEN (t.current_market_value_value - (t.price_value * t.quantity))
                ELSE NULL 
            END AS ProfitLoss,
            CASE 
                WHEN t.type = 'Buy' AND t.current_market_value_value IS NOT NULL AND (t.price_value * t.quantity) > 0
                THEN ((t.current_market_value_value - (t.price_value * t.quantity)) / (t.price_value * t.quantity) * 100)
                ELSE NULL 
            END AS ProfitLossPercentage
        FROM transactions t
        INNER JOIN portfolios p ON p.id = t.portfolio_id
        WHERE t.portfolio_id = @PortfolioId AND p.user_id = @UserId
        ORDER BY t.transaction_date DESC
        LIMIT @PageSize OFFSET @Offset
        """;

        var parameters = new
        {
            PortfolioId = portfolioId,
            UserId = userId,
            Offset = offset,
            paginatedParams.PageSize
        };

        using var connection = dbContext.Database.GetDbConnection();
        var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);
        var transactions = await connection.QueryAsync<PortfolioTransactionsResponseDto>(transactionsSql, parameters);

        return new PaginatedResult<PortfolioTransactionsResponseDto>(
            paginatedParams.PageNumber,
            paginatedParams.PageSize,
            totalCount,
            [.. transactions]
        );
    }
}
