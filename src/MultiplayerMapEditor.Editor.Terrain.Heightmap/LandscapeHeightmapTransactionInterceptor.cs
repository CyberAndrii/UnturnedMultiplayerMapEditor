using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SDG.Framework.Devkit.Transactions;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// Injects custom logic into the <see cref="SDG.Framework.Landscapes.LandscapeHeightmapTransaction"/> class.
/// </summary>
internal sealed class LandscapeHeightmapTransactionInterceptor : IHostedService
{
    private readonly INetHeightmapManager _heightmapManager;
    private Harmony? _harmony;

    private delegate void Redo(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        float[,] differences
    );

    private delegate void Undo(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        float[,] differences
    );

    private delegate void Begin(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        ref float[,]? differences
    );

    private delegate void End(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        ref float[,]? differences
    );

    private static event Redo? OnRedoPrefix;
    private static event Undo? OnUndoPrefix;
    private static event Begin? OnBeginPrefix;
    private static event End? OnEndPrefix;

    public LandscapeHeightmapTransactionInterceptor(INetHeightmapManager heightmapManager)
    {
        _heightmapManager = heightmapManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _harmony = Harmony.CreateAndPatchAll(GetType());
        OnRedoPrefix += HandleRedoPrefix;
        OnUndoPrefix += HandleUndoPrefix;
        OnBeginPrefix += HandleBeginPrefix;
        OnEndPrefix += HandleEndPrefix;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        OnRedoPrefix -= HandleRedoPrefix;
        OnUndoPrefix -= HandleUndoPrefix;
        OnBeginPrefix -= HandleBeginPrefix;
        OnEndPrefix -= HandleEndPrefix;
        _harmony?.UnpatchSelf();
        return Task.CompletedTask;
    }

    private void HandleRedoPrefix(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        float[,] differences)
    {
        _heightmapManager.ApplyDifference(tile, differences);
    }

    private void HandleUndoPrefix(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        float[,] differences)
    {
        _heightmapManager.UnapplyDifference(tile, differences);
    }

    private void HandleBeginPrefix(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        ref float[,]? differences)
    {
        _heightmapManager.BeginTransaction(tile.coord);
    }

    private void HandleEndPrefix(
        LandscapeHeightmapTransaction instance,
        LandscapeTile tile,
        // ReSharper disable once RedundantAssignment // intended behaviour
        ref float[,]? differences)
    {
        differences = _heightmapManager.EndTransaction(tile.coord);
    }

    [HarmonyPatch(
        typeof(LandscapeHeightmapTransaction),
        nameof(LandscapeHeightmapTransaction.redo)
    )]
    [HarmonyPrefix]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static bool RedoPrefix(
        LandscapeHeightmapTransaction __instance,
        LandscapeTile? ___tile,
        float[,]? ___heightmapCopy)
    {
        if (___tile == null || ___heightmapCopy == null)
        {
            return false;
        }

        OnRedoPrefix?.Invoke(__instance, ___tile, ___heightmapCopy);
        return false; // false - prevent running the original method
    }

    [HarmonyPatch(
        typeof(LandscapeHeightmapTransaction),
        nameof(LandscapeHeightmapTransaction.undo)
    )]
    [HarmonyPrefix]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static bool UndoPrefix(
        LandscapeHeightmapTransaction __instance,
        LandscapeTile? ___tile,
        float[,]? ___heightmapCopy)
    {
        if (___tile == null || ___heightmapCopy == null)
        {
            return false;
        }

        OnUndoPrefix?.Invoke(__instance, ___tile, ___heightmapCopy);
        return false; // false - prevent running the original method
    }

    [HarmonyPatch(
        typeof(LandscapeHeightmapTransaction),
        nameof(LandscapeHeightmapTransaction.begin)
    )]
    [HarmonyPrefix]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static bool BeginPrefix(
        LandscapeHeightmapTransaction __instance,
        LandscapeTile? ___tile,
        ref float[,]? ___heightmapCopy)
    {
        if (___tile == null)
        {
            return false;
        }

        OnBeginPrefix?.Invoke(__instance, ___tile, ref ___heightmapCopy);

        return false; // false - prevent running the original method
    }

    [HarmonyPatch(
        typeof(LandscapeHeightmapTransaction),
        nameof(LandscapeHeightmapTransaction.end)
    )]
    [HarmonyPrefix]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static bool EndPrefix(
        LandscapeHeightmapTransaction __instance,
        LandscapeTile? ___tile,
        ref float[,]? ___heightmapCopy)
    {
        if (___tile == null)
        {
            return false;
        }

        OnEndPrefix?.Invoke(__instance, ___tile, ref ___heightmapCopy);

        return false; // false - prevent running the original method
    }
}
