using FluentValidation.Results;

namespace FinnHub.Shared.Core.Extensions;
public static class ValidationResultExtensions
{
    public static ValidationError GetError(this ValidationResult result)
         => new([.. result.Errors.Select(e => Error.Problem(e.ErrorCode, e.ErrorMessage))]);
}
