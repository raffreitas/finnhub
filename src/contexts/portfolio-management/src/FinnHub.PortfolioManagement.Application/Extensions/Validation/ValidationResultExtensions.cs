using FinnHub.Shared.Core;

using FluentValidation.Results;

namespace FinnHub.PortfolioManagement.Application.Extensions.Validation;
internal static class ValidationResultExtensions
{
    public static ValidationError GetError(this ValidationResult result)
        => new([.. result.Errors.Select(e => Error.Problem(e.ErrorCode, e.ErrorMessage))]);
}
