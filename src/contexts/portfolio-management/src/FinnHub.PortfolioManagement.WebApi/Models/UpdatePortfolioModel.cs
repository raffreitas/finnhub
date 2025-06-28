using FinnHub.PortfolioManagement.Application.Commands.UpdatePortfolio;

namespace FinnHub.PortfolioManagement.WebApi.Models;

public sealed record UpdatePortfolioModel
{
    public string? Name { get; init; }
    public string? Description { get; init; }

    public UpdatePortfolioRequest ToRequest(Guid id) => new()
    {
        Id = id,
        Name = Name,
        Description = Description
    };
}
