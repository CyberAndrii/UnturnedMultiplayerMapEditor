using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.UI;

/// <summary>
/// Injects current ping into the Debug UI.
/// </summary>
public sealed class DebugUI : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private readonly IClientsideNetManager _netManager;
    private readonly CancellationTokenSource _cts = new();
    private Harmony? _harmony;

    private static DebugUI? _instance;

    public DebugUI(ILogger<DebugUI> logger, IClientsideNetManager netManager)
    {
        _logger = logger;
        _netManager = netManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"Another instance of {nameof(DebugUI)} already exists.");
        }

        _instance = this;
        _harmony = Harmony.CreateAndPatchAll(GetType());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _instance = null;
        _cts.Cancel();
        _harmony?.UnpatchSelf();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    private void InjectPing(StringBuilder stringBuilder)
    {
        if (_netManager.ServerPeer == null)
        {
            return;
        }

        var debugString = stringBuilder.ToString();
        var indexOfFirstSpace = debugString.IndexOf(' ');

        if (indexOfFirstSpace == -1)
        {
            stringBuilder.Append(' ');
            stringBuilder.Append(_netManager.ServerPeer.Ping);
            stringBuilder.Append("ms");
            return;
        }

        stringBuilder.Clear();
        stringBuilder.Append(debugString.Substring(0, indexOfFirstSpace + 1));
        stringBuilder.Append(_netManager.ServerPeer.Ping);
        stringBuilder.Append("ms");
        stringBuilder.Append(debugString.Substring(indexOfFirstSpace));
    }

    [HarmonyPatch(
        "SDG.Unturned.GlazierBase, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
        "UpdateDebugString"
    )]
    [HarmonyPostfix]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static void UpdateDebugStringPostfix(StringBuilder ___debugBuilder)
    {
        _instance?.InjectPing(___debugBuilder);
    }
}
