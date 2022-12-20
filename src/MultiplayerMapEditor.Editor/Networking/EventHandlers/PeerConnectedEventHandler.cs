using MultiplayerMapEditor.Abstractions.Level;
using MultiplayerMapEditor.Editor.Level;
using MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

namespace MultiplayerMapEditor.Editor.Networking.EventHandlers;

internal sealed class ServersidePeerConnectedEventHandler : IPeerConnectedEventHandler
{
    private readonly IPeerConnectedEventHandler _peerConnectedEventHandler;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelManager _levelManager;

    public ServersidePeerConnectedEventHandler(
        IPeerConnectedEventHandler peerConnectedEventHandler,
        INetPacketProcessor netPacketProcessor,
        ILevelManager levelManager)
    {
        _peerConnectedEventHandler = peerConnectedEventHandler;
        _netPacketProcessor = netPacketProcessor;
        _levelManager = levelManager;
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _peerConnectedEventHandler.OnPeerConnected(peer); // call decorated class

        _netPacketProcessor.SendNetSerializable(
            peer,
            new LoadLevelCommand { Name = _levelManager.CurrentLevelInfo!.Name },
            DeliveryMethod.ReliableSequenced
        );
    }
}
