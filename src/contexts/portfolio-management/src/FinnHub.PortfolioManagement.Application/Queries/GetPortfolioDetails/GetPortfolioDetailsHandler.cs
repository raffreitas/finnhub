using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.PortfolioManagement.Domain.Aggregates.Entities;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.Shared.Core;
using FinnHub.Shared.Core.Extensions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfolioDetails;

internal sealed class GetPortfolioDetailsHandler(
    ILogger<GetPortfolioDetailsHandler> logger,
    IUserContext userContext,
    IPortfolioRepository portfolioRepository
) : IRequestHandler<GetPortfolioDetailsRequest, Result<GetPortfolioDetailsResponse>>
{
    public async Task<Result<GetPortfolioDetailsResponse>> Handle(GetPortfolioDetailsRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<GetPortfolioDetailsResponse>.ValidationFailure(validationResult.GetError());

        var portfolio = await portfolioRepository.GetByIdAsync(userContext.UserId, request.Id, cancellationToken);

        if (portfolio is null)
        {
            logger.LogWarning("User with id {UserId} request a portfolio with Id {PortfolioId} but is not found.", userContext.UserId, request.Id);
            return Result.Failure<GetPortfolioDetailsResponse>(PortfolioErrors.PortfolioNotFound);
        }

        var totalValue = portfolio.CalculateCurrentValue();
        var totalInvested = portfolio.CalculateTotalCostBasis();
        var totalUnrealizedPnL = totalValue.Value - totalInvested.Value;
        var totalUnrealizedPnLPercentage = totalInvested.Value != 0
            ? (totalUnrealizedPnL / totalInvested.Value) * 100
            : 0;

        return new GetPortfolioDetailsResponse
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            Description = portfolio.Description,
            UserId = portfolio.UserId,
            TotalValue = totalValue.Value,
            TotalInvested = totalInvested.Value,
            TotalUnrealizedPnL = totalUnrealizedPnL,
            TotalUnrealizedPnLPercentage = totalUnrealizedPnLPercentage,
            CreatedAt = portfolio.CreationDate.DateTime,
            Positions = [.. portfolio.Positions.Select(MapPositionToResponse)]
        };
    }

    private static PositionResponse MapPositionToResponse(Position position)
    {
        decimal? unrealizedPnL = position.CurrentMarketValue is not null
            ? position.CurrentMarketValue.Value - position.TotalCost.Value
            : null;

        decimal? unrealizedPnLPercentage = position.CurrentMarketValue is not null && position.TotalCost.Value > 0
                ? ((position.CurrentMarketValue.Value - position.TotalCost.Value) / position.TotalCost.Value) * 100
                : null;

        return new PositionResponse
        {
            Id = position.Id,
            Symbol = position.AssetSymbol.Value,
            Quantity = position.Quantity.Value,
            AveragePrice = position.AverageCost?.Value ?? 0,
            TotalCost = position.TotalCost.Value,
            CurrentMarketPrice = position.CurrentMarketPrice?.Value,
            CurrentMarketValue = position.CurrentMarketValue?.Value,
            UnrealizedPnL = unrealizedPnL,
            UnrealizedPnLPercentage = unrealizedPnLPercentage
        };
    }
}
