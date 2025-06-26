namespace FinnHub.PortfolioManagement.Application.Abstractions.Tokens;

public interface ITokenService
{
    public Task<string> GetAccessToken();
}
