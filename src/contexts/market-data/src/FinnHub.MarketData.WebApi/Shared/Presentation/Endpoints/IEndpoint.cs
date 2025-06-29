using Microsoft.AspNetCore.Routing;

namespace FinnHub.MarketData.WebApi.Shared.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);

}
