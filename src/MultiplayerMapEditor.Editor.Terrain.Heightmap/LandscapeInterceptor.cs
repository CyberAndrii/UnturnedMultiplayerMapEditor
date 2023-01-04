using HarmonyLib;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// Injects custom logic into the <see cref="SDG.Framework.Landscapes.Landscape"/> class.
/// </summary>
internal sealed class LandscapeInterceptor : IHostedService
{
    private readonly INetHeightmapManager _heightmapManager;
    private Harmony? _harmony;

    public LandscapeInterceptor(INetHeightmapManager heightmapManager)
    {
        _heightmapManager = heightmapManager;
    }

    private delegate void WriteHeightmap(
        Bounds worldBounds,
        ref Landscape.LandscapeWriteHeightmapHandler callback
    );

    private static event WriteHeightmap? OnWriteHeightmapPrefix;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _harmony = Harmony.CreateAndPatchAll(GetType());
        OnWriteHeightmapPrefix += HandleWriteHeightmapPrefix;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        OnWriteHeightmapPrefix -= HandleWriteHeightmapPrefix;
        _harmony?.UnpatchSelf();
        return Task.CompletedTask;
    }

    private void HandleWriteHeightmapPrefix(
        Bounds worldBounds,
        ref Landscape.LandscapeWriteHeightmapHandler callback)
    {
        _heightmapManager.OnWriteHeightmap(worldBounds, ref callback);
    }

    [HarmonyPatch(
        typeof(Landscape),
        nameof(Landscape.writeHeightmap),
        typeof(Bounds), typeof(Landscape.LandscapeWriteHeightmapHandler)
    )]
    [HarmonyPrefix]
    private static bool WriteHeightmapPrefix(
        Bounds worldBounds,
        ref Landscape.LandscapeWriteHeightmapHandler callback)
    {
        OnWriteHeightmapPrefix?.Invoke(worldBounds, ref callback);
        return true; // true - run the original method
    }
}
