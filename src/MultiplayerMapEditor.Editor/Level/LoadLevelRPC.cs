using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Abstractions;
using MultiplayerMapEditor.Abstractions.Level;

namespace MultiplayerMapEditor.Editor.Level;

internal sealed class LoadLevelCommand : INetSerializable
{
    public string Name { get; set; } = "";

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
    }
}

internal sealed class LoadLevelRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly IConnectionManager _connectionManager;
    private readonly ILevelManager _levelManager;

    public LoadLevelRPC(
        INetPacketProcessor netPacketProcessor,
        IConnectionManager connectionManager,
        ILevelManager levelManager)
    {
        _netPacketProcessor = netPacketProcessor;
        _connectionManager = connectionManager;
        _levelManager = levelManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<LoadLevelCommand>(OnReceiveLoadLevel);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<LoadLevelCommand>();
        return Task.CompletedTask;
    }

    private void OnReceiveLoadLevel(LoadLevelCommand packet)
    {
        var level = _levelManager.Find(packet.Name);

        if (level == null)
        {
            _connectionManager.StopAsync($"""Map "{packet.Name}" was not found""").Forget();
            return;
        }

        // This RPC can only be called once, so unsubscribe
        _netPacketProcessor.RemoveSubscription<LoadLevelCommand>();

        _levelManager.LoadLevel(level);
    }
}
