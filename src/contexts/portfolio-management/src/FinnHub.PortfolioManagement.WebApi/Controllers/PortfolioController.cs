using System.Net;

using FinnHub.PortfolioManagement.Application.Commands.CreatePortfolio;
using FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;
using FinnHub.PortfolioManagement.Application.Commands.RegisterSellAsset;
using FinnHub.PortfolioManagement.Application.Queries.GetPortfoliosSummaryList;
using FinnHub.PortfolioManagement.WebApi.Models;

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

    [HttpPost("{id:guid}/transactions/buy")]
    [ProducesResponseType<RegisterBuyAssetResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterBuyTransaction(
        [FromRoute] Guid id,
        [FromBody] RegisterTransactionModel request,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request.ToBuyAssetRequest(id), cancellationToken);

        return result.IsSuccess
            ? HandleResponse(result, HttpStatusCode.Created)
            : HandleResponse(result);
    }

    [HttpPost("{id:guid}/transactions/sell")]
    [ProducesResponseType<RegisterSellAssetResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterSellTransaction(
        [FromRoute] Guid id,
        [FromBody] RegisterTransactionModel request,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request.ToSellAssetRequest(id), cancellationToken);

        return result.IsSuccess
            ? HandleResponse(result, HttpStatusCode.Created)
            : HandleResponse(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType<GetPortfoliosSummaryListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPortfoliosSummaryList(
        [FromQuery] GetPortfoliosSummaryListRequest query,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess
            ? HandleResponse(result, HttpStatusCode.OK)
            : HandleResponse(result);
    }
}
