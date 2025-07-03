using FinnHub.Shared.Core;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;

public sealed record RegisterBuyAssetRequest : IRequest<Result<RegisterBuyAssetResponse>>
{
    public required Guid PortfolioId { get; init; }
    public required string AssetSymbol { get; set; }
    public required int Quantity { get; set; }
    public required decimal PricePerUnit { get; set; }
    public required DateTimeOffset TransactionDate { get; set; }

    public ValidationResult Validate() => new RegisterBuyAssetRequestValidation().Validate(this);
};

internal sealed class RegisterBuyAssetRequestValidation : AbstractValidator<RegisterBuyAssetRequest>
{
    public RegisterBuyAssetRequestValidation()
    {
        RuleFor(x => x.PortfolioId)
            .NotEmpty();

        RuleFor(x => x.AssetSymbol)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(10);

        RuleFor(x => x.Quantity)
            .GreaterThan(1);

        RuleFor(x => x.PricePerUnit)
            .GreaterThan(0);

        RuleFor(x => x.TransactionDate)
            .NotEmpty();
    }
}
