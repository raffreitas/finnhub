using FinnHub.MarketData.WebApi.Shared.Domain.Entities;
using FinnHub.MarketData.WebApi.Shared.Domain.Enums;

namespace FinnHub.MarketData.WebApi.Features.Assets.Domain.Entities;

public class Asset : Entity
{
    public string Symbol { get; init; }
    public string Name { get; private set; }
    public AssetType Type { get; private set; }
    public string Exchange { get; init; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public Asset(string symbol, string name, AssetType type, string exchange)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(symbol));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(exchange));
        Symbol = symbol.ToUpper();
        Name = name;
        Exchange = exchange;
        Type = type;
        IsActive = true;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Asset name cannot be empty", nameof(newName));

        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
