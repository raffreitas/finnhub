using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace FinnHub.PortfolioManagement.WebApi.Controllers;

[ApiController]
public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    public HashSet<string> Errors { get; private set; } = [];

    protected ActionResult HandleResponse(HttpStatusCode statusCode, IEnumerable<string>? errors = null, object? response = null)
    {
        foreach (var error in errors ?? [])
            Errors.Add(error);

        return IsValid() ? CustomResponse(statusCode, response) : CustomResponse(statusCode);
    }

    private ObjectResult CustomResponse(HttpStatusCode statusCode, object? response = null) => statusCode switch
    {
        HttpStatusCode.NotFound => NotFound(new CustomProblemDetails(statusCode, errors: Errors)),
        HttpStatusCode.BadRequest => BadRequest(new CustomProblemDetails(statusCode, errors: Errors)),
        HttpStatusCode.UnprocessableEntity => BadRequest(new CustomProblemDetails(statusCode, errors: Errors)),
        HttpStatusCode.Created when IsValid() => StatusCode(StatusCodes.Status201Created, response),
        HttpStatusCode.Created => BadRequest(new CustomProblemDetails(statusCode, errors: Errors)),
        _ => IsValid() ? Ok(response) : BadRequest(new CustomProblemDetails(statusCode, errors: Errors))
    };

    private bool IsValid() => Errors.Count == 0;

    class CustomProblemDetails : ProblemDetails
    {
        public IEnumerable<string>? Errors { get; private init; }
        public CustomProblemDetails(HttpStatusCode statusCode, IEnumerable<string>? errors)
        {
            Title = statusCode switch
            {
                HttpStatusCode.UnprocessableEntity => "Validation Error",
                HttpStatusCode.BadRequest => "Bad Request",
                _ => "An error occurred"
            };

            Errors = errors is null ? default : errors;
        }
    }
}
