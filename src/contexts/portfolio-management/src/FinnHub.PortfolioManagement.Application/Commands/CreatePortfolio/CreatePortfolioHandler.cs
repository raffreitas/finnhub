using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Extensions.Validation;
using FinnHub.PortfolioManagement.Application.Shared.Result;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;

internal sealed class CreatePortfolioHandler : IRequestHandler<CreatePortfolioRequest, Result<CreatePortfolioResponse>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreatePortfolioHandler(
        IPortfolioRepository portfolioRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext
    )
    {
        _portfolioRepository = portfolioRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<CreatePortfolioResponse>> Handle(CreatePortfolioRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return Result<CreatePortfolioResponse>.Failure(validationResult.GetErrors());

        var currentUserId = _userContext.UserId;

        var nameIsTaken = await _portfolioRepository.NameExistsAsync(currentUserId, request.Name, cancellationToken);
        if (nameIsTaken)
            return Result<CreatePortfolioResponse>.Failure("A portfolio with this name already exists.");

        var portfolio = Portfolio.Create(currentUserId, request.Name, request.Description);

        await _portfolioRepository.AddAsync(portfolio, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CreatePortfolioResponse(Guid.Empty));
    }
}
