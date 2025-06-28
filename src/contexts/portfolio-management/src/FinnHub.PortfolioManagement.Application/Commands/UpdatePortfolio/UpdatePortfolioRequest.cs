using FinnHub.Shared.Core;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.UpdatePortfolio;
public sealed record UpdatePortfolioRequest : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }

    public ValidationResult Validate() => new UpdatePortfolioRequestValidation().Validate(this);
}

internal sealed class UpdatePortfolioRequestValidation : AbstractValidator<UpdatePortfolioRequest>
{
    public UpdatePortfolioRequestValidation()
    {
        RuleFor(x => x.Id).NotEmpty();

        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                            .NotEmpty()
                            .MinimumLength(3)
                            .MaximumLength(50);
        });

        When(x => !string.IsNullOrEmpty(x.Description), () =>
        {
            RuleFor(x => x.Description).MaximumLength(500);
        });
    }
}
