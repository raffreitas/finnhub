﻿using FinnHub.Shared.Core;

namespace FinnHub.PortfolioManagement.Application.Errors;
internal static class PortfolioErrors
{
    public static readonly Error PortfolioNameNotUnique = Error.Conflict(
        "Portfolio.NameNotUnique",
        "A portfolio with this name already exists."
    );

    public static readonly Error PortfolioNotFound = Error.NotFound(
        "Portfolio.NotFound",
        "The specified portfolio was not found."
    );
}
