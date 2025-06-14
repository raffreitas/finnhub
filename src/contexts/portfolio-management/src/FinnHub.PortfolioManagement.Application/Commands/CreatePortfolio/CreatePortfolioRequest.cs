using FinnHub.PortfolioManagement.Application.Shared.Result;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;

public sealed record CreatePortfolioRequest : IRequest<Result<CreatePortfolioResponse>>
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    public ValidationResult Validate() => new CreatePortfolioRequestValidation().Validate(this);
}

public sealed class CreatePortfolioRequestValidation : AbstractValidator<CreatePortfolioRequest>
{
    public CreatePortfolioRequestValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
