namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface IPeerDisconnectedEventHandler
{
    void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);
}
