using System.Net;

using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinnHub.PortfolioManagement.WebApi.Controllers;

[Route("api/v1/portfolios")]
[Authorize]
public class PortfolioController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreatePortfolioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreatePortfolio(
        [FromBody] CreatePortfolioRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return result.IsSuccess 
            ? HandleResponse(result, HttpStatusCode.Created) 
            : HandleResponse(result);
    }
}
