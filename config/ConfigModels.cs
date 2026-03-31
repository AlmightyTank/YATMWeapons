using System.Text.Json.Serialization;

namespace YATMWeapons.config;

public class SettingsConfig
{
    public int MinLevel { get; set; } = 1;
    public bool UnlockedByDefault { get; set; } = false;
    
    // Refresh
    public int TraderRefreshMin { get; set; } = 1800;
    public int TraderRefreshMax { get; set; } = 3600;

    // Flea & Services
    public bool AddTraderToFleaMarket { get; set; } = true;
    public int InsurancePriceCoef { get; set; } = 25;
    public double RepairQuality { get; set; } = 0.8;

    // Stock & Pricing
    public bool RandomizeStockAvailable { get; set; } = true;
    public int OutOfStockChance { get; set; } = 15;
    public bool UnlimitedStock { get; set; } = false;
    public double PriceMultiplier { get; set; } = 1.0;
    
    // Debug
    public bool DebugLogging { get; set; } = false;

}

public class PriceConfigItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? _Comment { get; set; } // For user readability

    public string TplId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Currency { get; set; } = "RUB";
}
