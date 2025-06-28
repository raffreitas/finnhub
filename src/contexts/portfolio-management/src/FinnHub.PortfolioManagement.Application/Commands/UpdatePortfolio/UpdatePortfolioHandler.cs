using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.Shared.Core;
using FinnHub.Shared.Core.Extensions;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.UpdatePortfolio;

internal class UpdatePortfolioHandler(
    IPortfolioRepository portfolioRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
)
: IRequestHandler<UpdatePortfolioRequest, Result>
{
    public async Task<Result> Handle(UpdatePortfolioRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<object>.ValidationFailure(validationResult.GetError()); // TODO: Add non generic validation result

        var portfolio = await portfolioRepository.GetByIdAsync(userContext.UserId, request.Id, cancellationToken);

        if (portfolio is null)
            return Result.Failure(PortfolioErrors.PortfolioNotFound);

        if (!string.IsNullOrEmpty(request.Name))
            portfolio.Rename(request.Name);

        if (!string.IsNullOrEmpty(request.Description))
            portfolio.UpdateDescription(request.Description);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
