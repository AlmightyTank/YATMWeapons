using CommonLibExtended.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using System.Reflection;
using Range = SemanticVersioning.Range;

namespace YATMWeapons.src;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.amightytank.yatmweapons";
    public override string Name { get; init; } = "YATMWeapons";
    public override string Author { get; init; } = "AmightyTank";
    public override List<string>? Contributors { get; init; } = [];
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.11");
    public override List<string>? Incompatibilities { get; init; } = [];
    public override Dictionary<string, Range>? ModDependencies { get; init; } = new()
    {
        { "com.amightytank.commonlibextended", new Range("~1.0.0") },
        { "com.wtt.commonlib", new Range("~2.0.15") }
    };
    public override string? Url { get; init; } = null;
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 6)]
public sealed class CustomContentLoader(
    WTTServerCommonLib.WTTServerCommonLib wttCommon,
    CommonLibExtendedBootstrap commonLibExtendedBootstrap) : IOnLoad
{
    public async Task OnLoad()
    {
        try
        {
            YATMLogger.Log("[CustomContentLoader] Starting custom content load...");

            var assembly = Assembly.GetExecutingAssembly();
            var modPath = Path.GetDirectoryName(assembly.Location)
                ?? throw new InvalidOperationException("Could not resolve mod path.");

            var dbPath = Path.Combine(modPath, "db");

            YATMLogger.LogDebug($"[CustomContentLoader] Mod path: {modPath}");
            YATMLogger.LogDebug($"[CustomContentLoader] DB path: {dbPath}");

            if (!Directory.Exists(dbPath))
            {
                YATMLogger.Log($"[CustomContentLoader] DB folder not found: {dbPath}");
                return;
            }

            var itemPaths = new[]
            {
                Path.Join("db", "CustomAmmo"),
                Path.Join("db", "CustomArmor"),
                Path.Join("db", "CustomParts"),
                Path.Join("db", "CustomWeapons")
            };

            var slotCopyPaths = new[]
            {
                Path.Join("db", "CustomArmor"),
                Path.Join("db", "CustomParts"),
                Path.Join("db", "CustomWeapons")
            };

            var presetPath = Path.Join("db", "CustomWeaponPresets");

            YATMLogger.LogDebug("[CustomContentLoader] Loading WTT items...");

            foreach (var path in itemPaths)
            {
                await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly, path);
            }

            foreach (var path in slotCopyPaths)
            {
                await commonLibExtendedBootstrap.ProcessSlotCopies(assembly, path);
            }

            await wttCommon.CustomWeaponPresetService.CreateCustomWeaponPresets(assembly, presetPath);
            await commonLibExtendedBootstrap.RegisterWeaponPresets(assembly, presetPath);

            YATMLogger.Log("[CustomContentLoader] Finished loading WTT items and presets.");

            foreach (var path in itemPaths)
            {
                await commonLibExtendedBootstrap.ProcessTheRest(assembly, path);
            }

            YATMLogger.Log("[CustomContentLoader] Finished loading CommonLibExtended items.");
            YATMLogger.Log("[CustomContentLoader] Finished loading all custom content.");
        }
        catch (Exception ex)
        {
            YATMLogger.Log($"[CustomContentLoader] Exception during content load: {ex}");
            YATMLogger.LogDebug(ex.StackTrace ?? "No stack trace");
        }
    }
}