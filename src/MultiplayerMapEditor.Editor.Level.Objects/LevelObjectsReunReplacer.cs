using HarmonyLib;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Editor.Level.Objects.Create;
using MultiplayerMapEditor.Editor.Level.Objects.Remove;
using MultiplayerMapEditor.Editor.Level.Objects.Transform;

namespace MultiplayerMapEditor.Editor.Level.Objects;

internal sealed class LevelObjectsReunReplacer : IHostedService
{
    private readonly INetObjectCreator _netObjectCreator;
    private readonly INetObjectRemover _netObjectRemover;
    private readonly INetObjectTransformer _netObjectTransformer;
    private readonly ILogger<LevelObjectsReunReplacer> _logger;
    private Harmony? _harmony;

    private delegate void RegisterReun(ref IReun reun);

    private static event RegisterReun? OnRegisterReunPrefix;

    public LevelObjectsReunReplacer(
        INetObjectCreator netObjectCreator,
        INetObjectRemover netObjectRemover,
        INetObjectTransformer netObjectTransformer,
        ILogger<LevelObjectsReunReplacer> logger)
    {
        _netObjectCreator = netObjectCreator;
        _netObjectRemover = netObjectRemover;
        _netObjectTransformer = netObjectTransformer;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        OnRegisterReunPrefix += HandleRegisterReunPrefix;
        _harmony = Harmony.CreateAndPatchAll(GetType());
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        OnRegisterReunPrefix -= HandleRegisterReunPrefix;
        _harmony?.UnpatchSelf();
        return Task.CompletedTask;
    }

    private void HandleRegisterReunPrefix(ref IReun reun)
    {
        var originalReun = reun;

        reun = reun switch
        {
            ReunObjectAdd add => NetReunObjectAdd.From(add, _netObjectCreator, _netObjectRemover),
            ReunObjectRemove remove => NetReunObjectRemove.From(remove, _netObjectCreator, _netObjectRemover),
            ReunObjectTransform transform => NetReunObjectTransform.From(transform, _netObjectTransformer),
            _ => reun
        };

        if (reun == originalReun)
        {
            _logger.LogReunNotReplaced(reun.GetType().FullName);
            return;
        }

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogReunReplaced(originalReun.GetType().Name, reun.GetType().Name);
        }
    }

    [HarmonyPatch(
        typeof(LevelObjects),
        nameof(LevelObjects.register),
        typeof(IReun)
    )]
    [HarmonyPrefix]
    private static void RegisterReunPrefix(ref IReun newReun)
    {
        OnRegisterReunPrefix?.Invoke(ref newReun);
    }
}
