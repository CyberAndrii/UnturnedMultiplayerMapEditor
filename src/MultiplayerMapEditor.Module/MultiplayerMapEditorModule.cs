using System.Reflection;
using Cysharp.Threading.Tasks;
using MultiplayerMapEditor.HostBuilder;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace MultiplayerMapEditor.Module;

/// <summary>
/// Main entrypoint.
/// Instantiated and used by the game via reflection.
/// </summary>
public sealed class MultiplayerMapEditorModule : IModuleNexus
{
    public IBootstrapper? Bootstrapper { get; private set; }

    public MultiplayerMapEditorModule()
    {
        var moduleDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.Parent!.FullName;
        Bootstrapper = new Bootstrapper(moduleDirectory, ThreadUtil.gameThread.ManagedThreadId);
    }

    public void initialize()
    {
        Bootstrapper!.StartAsync(CancellationToken.None).Forget();
    }

    public void shutdown()
    {
        Bootstrapper!.StopAsync(CancellationToken.None).Forget();
        Bootstrapper = null;
    }
}
