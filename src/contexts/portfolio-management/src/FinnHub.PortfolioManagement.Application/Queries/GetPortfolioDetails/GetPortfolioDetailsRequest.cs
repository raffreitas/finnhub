using FinnHub.Shared.Core;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Queries.GetPortfolioDetails;

public sealed record GetPortfolioDetailsRequest : IRequest<Result<GetPortfolioDetailsResponse>>
{
    public required Guid Id { get; init; }

    public ValidationResult Validate() => new GetPortfolioDetailsValidation().Validate(this);
}

internal sealed class GetPortfolioDetailsValidation : AbstractValidator<GetPortfolioDetailsRequest>
{
    public GetPortfolioDetailsValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
