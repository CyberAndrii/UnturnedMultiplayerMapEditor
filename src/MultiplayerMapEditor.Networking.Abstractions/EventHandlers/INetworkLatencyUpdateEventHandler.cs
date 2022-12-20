namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface INetworkLatencyUpdateEventHandler
{
    void OnNetworkLatencyUpdate(NetPeer peer, int latency);
}
