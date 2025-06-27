using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.Shared.Core;
using FinnHub.Shared.Core.Extensions;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;

internal sealed class RegisterBuyAssetHandler(
    IPortfolioRepository portfolioRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMarketDataService marketDataService
) : IRequestHandler<RegisterBuyAssetRequest, Result<RegisterBuyAssetResponse>>
{
    public async Task<Result<RegisterBuyAssetResponse>> Handle(RegisterBuyAssetRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<RegisterBuyAssetResponse>.ValidationFailure(validationResult.GetError());

        var portfolio = await portfolioRepository.GetByIdAsync(userContext.UserId, request.PortfolioId, cancellationToken);

        if (portfolio is null)
            return Result.Failure<RegisterBuyAssetResponse>(PortfolioErrors.PortfolioNotFound);

        var currentMarketValueResult = await marketDataService
            .GetCurrentMarketValueAsync(request.AssetSymbol, cancellationToken);

        if (currentMarketValueResult.IsFailure)
            return Result.Failure<RegisterBuyAssetResponse>(currentMarketValueResult.Error);

        var transaction = portfolio.BuyAsset(
              request.AssetSymbol,
              request.Quantity,
              request.PricePerUnit,
              currentMarketValueResult.Value,
              request.TransactionDate
        );

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegisterBuyAssetResponse(transaction.Id));
    }
}
