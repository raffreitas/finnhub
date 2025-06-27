namespace FinnHub.PortfolioManagement.Application.DTOs;

public sealed record PortfolioSummaryResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreationDate,
    decimal TotalValue,
    decimal TotalProfit
);

