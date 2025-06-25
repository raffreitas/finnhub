using FinnHub.PortfolioManagement.Application.Abstractions;
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
    IUserContext userContext
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

        // TODO: Implement logic to call market data service to get the current market value of the asset
        var currentMarketValue = 0;

        var transaction = portfolio.BuyAsset(
              request.AssetSymbol,
              request.Quantity,
              request.PricePerUnit,
              currentMarketValue,
              request.TransactionDate
        );

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegisterBuyAssetResponse(transaction.Id));
    }
}
