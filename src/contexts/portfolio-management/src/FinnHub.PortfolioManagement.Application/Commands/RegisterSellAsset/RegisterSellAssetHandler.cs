using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.Shared.Core;
using FinnHub.Shared.Core.Extensions;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.RegisterSellAsset;

internal sealed class RegisterSellAssetHandler(
    IPortfolioRepository portfolioRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext
) : IRequestHandler<RegisterSellAssetRequest, Result<RegisterSellAssetResponse>>
{
    public async Task<Result<RegisterSellAssetResponse>> Handle(RegisterSellAssetRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<RegisterSellAssetResponse>.ValidationFailure(validationResult.GetError());

        var portfolio = await portfolioRepository.GetByIdAsync(userContext.UserId, request.PortfolioId, cancellationToken);

        if (portfolio is null)
            return Result.Failure<RegisterSellAssetResponse>(PortfolioErrors.PortfolioNotFound);

        var transaction = portfolio.SellAsset(
              request.AssetSymbol,
              request.Quantity,
              request.PricePerUnit,
              request.TransactionDate
        );

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegisterSellAssetResponse(transaction.Id));
    }
}
