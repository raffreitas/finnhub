using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Errors;
internal static class PortfolioErrors
{
    public static readonly Error PortfolioNameNotUnique = Error.Problem(
        "Portfolio.NameNotUnique",
        "A portfolio with this name already exists."
    );
}
