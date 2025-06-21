using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Errors;
using FinnHub.PortfolioManagement.Application.Extensions.Validation;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.Shared.Core;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;

internal sealed class CreatePortfolioHandler(
    IPortfolioRepository portfolioRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext
) : IRequestHandler<CreatePortfolioRequest, Result<CreatePortfolioResponse>>
{
    public async Task<Result<CreatePortfolioResponse>> Handle(CreatePortfolioRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<CreatePortfolioResponse>.ValidationFailure(validationResult.GetError());

        var currentUserId = userContext.UserId;

        var nameIsTaken = await portfolioRepository.NameExistsAsync(currentUserId, request.Name, cancellationToken);
        if (nameIsTaken)
            return Result.Failure<CreatePortfolioResponse>(PortfolioErrors.PortfolioNameNotUnique);

        var portfolio = Portfolio.Create(currentUserId, request.Name, request.Description);

        await portfolioRepository.AddAsync(portfolio, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CreatePortfolioResponse(portfolio.Id));
    }
}
