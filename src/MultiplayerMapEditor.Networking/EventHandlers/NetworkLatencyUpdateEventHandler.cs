using Microsoft.Extensions.Options;

namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class NetworkLatencyUpdateEventHandler : INetworkLatencyUpdateEventHandler
{
    private readonly INetManager _netManager;
    private readonly ServerOptions _serverOptions;

    public NetworkLatencyUpdateEventHandler(INetManager netManager, IOptions<ServerOptions> serverOptions)
    {
        _netManager = netManager;
        _serverOptions = serverOptions.Value;
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        if (_netManager is IServersideNetManager serversideNetManager && latency > _serverOptions.MaxLatency)
        {
            var disconnectReason = new Abstractions.Packets.DisconnectReason { Message = "Latency is too high" };
            serversideNetManager.Disconnect(peer, disconnectReason);
        }
    }
}
