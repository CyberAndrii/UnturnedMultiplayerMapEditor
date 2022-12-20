namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class PeerConnectedEventHandler : IPeerConnectedEventHandler
{
    private readonly ILogger<PeerConnectedEventHandler> _logger;

    public PeerConnectedEventHandler(ILogger<PeerConnectedEventHandler> logger)
    {
        _logger = logger;
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _logger.LogInformation("Peer {EndPoint} connected.", peer.EndPoint);
    }
}
