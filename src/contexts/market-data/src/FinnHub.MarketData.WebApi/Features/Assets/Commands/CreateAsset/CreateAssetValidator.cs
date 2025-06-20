using FluentValidation;

namespace FinnHub.MarketData.WebApi.Features.Assets.Commands.CreateAsset;

internal sealed class CreateAssetValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Symbol).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
    }
}
