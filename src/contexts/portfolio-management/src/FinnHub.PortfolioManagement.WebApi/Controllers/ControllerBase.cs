using System.Net;

using FinnHub.Shared.Core;

using Microsoft.AspNetCore.Mvc;

namespace FinnHub.PortfolioManagement.WebApi.Controllers;

[ApiController]
public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    protected ActionResult HandleResponse<T>(Result<T> result, HttpStatusCode? statusCode = HttpStatusCode.OK)
    {
        return result.IsSuccess
            ? CustomResponse(statusCode ?? HttpStatusCode.OK, result.Value)
            : CustomResponse(result.Error);
    }

    protected ActionResult HandleResponse(Result result, HttpStatusCode? statusCode = HttpStatusCode.OK)
    {
        return result.IsSuccess
            ? CustomResponse(statusCode ?? HttpStatusCode.OK)
            : CustomResponse(result.Error);
    }

    private ObjectResult CustomResponse<T>(HttpStatusCode statusCode, T? data = default)
        => StatusCode((int)statusCode, data);

    private StatusCodeResult CustomResponse(HttpStatusCode statusCode)
        => StatusCode((int)statusCode);

    private static ObjectResult CustomResponse(Error error)
        => new(CustomProblemDetails(error));

    private static ProblemDetails CustomProblemDetails(Error error) => new()
    {
        Title = GetErrorTitle(error),
        Detail = GetDetail(error),
        Type = GetType(error.Type),
        Status = GetStatusCode(error.Type),
        Extensions = GetErrors(error) ?? []
    };

    private static string GetErrorTitle(Error error) => error.Type switch
    {
        ErrorType.Validation => error.Code,
        ErrorType.Problem => error.Code,
        ErrorType.NotFound => error.Code,
        ErrorType.Conflict => error.Code,
        _ => "Server failure"
    };

    private static string GetDetail(Error error) => error.Type switch
    {
        ErrorType.Validation => error.Description,
        ErrorType.Problem => error.Description,
        ErrorType.NotFound => error.Description,
        ErrorType.Conflict => error.Description,
        _ => "An unexpected error occurred"
    };

    private static string GetType(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Problem => StatusCodes.Status422UnprocessableEntity,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static Dictionary<string, object?>? GetErrors(Error error)
        => error is not ValidationError validationError
            ? null
            : new Dictionary<string, object?> { { "errors", validationError.Errors } };
}
