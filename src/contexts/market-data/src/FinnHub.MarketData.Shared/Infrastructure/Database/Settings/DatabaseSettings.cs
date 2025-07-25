﻿using System.ComponentModel.DataAnnotations;

namespace FinnHub.MarketData.Shared.Infrastructure.Database.Settings;

internal sealed record DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";

    [Required, MinLength(1)]
    public required string ConnectionString { get; init; }

    [Required, MinLength(1)]
    public required string DatabaseName { get; init; }
}
