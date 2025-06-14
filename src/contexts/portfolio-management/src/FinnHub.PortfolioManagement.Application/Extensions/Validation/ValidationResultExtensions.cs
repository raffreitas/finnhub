using FluentValidation.Results;

namespace FinnHub.PortfolioManagement.Application.Extensions.Validation;
internal static class ValidationResultExtensions
{
    public static string[] GetErrors(this ValidationResult result)
        => [.. result.Errors.Select(e => e.ErrorMessage)];
}
