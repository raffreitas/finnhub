using FluentValidation.Results;

namespace FinnHub.MarketData.WebApi.Shared.Extensions;

internal static class ValidationResultExtensions
{
    public static string[] GetErrors(this ValidationResult result)
        => [.. result.Errors.Select(e => e.ErrorMessage)];
}
