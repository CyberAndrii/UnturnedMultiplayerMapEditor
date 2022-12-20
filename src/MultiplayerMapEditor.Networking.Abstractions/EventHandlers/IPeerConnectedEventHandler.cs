namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface IPeerConnectedEventHandler
{
    void OnPeerConnected(NetPeer peer);
}
