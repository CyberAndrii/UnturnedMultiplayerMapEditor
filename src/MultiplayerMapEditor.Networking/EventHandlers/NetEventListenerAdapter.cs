using System.Net;
using System.Net.Sockets;

namespace MultiplayerMapEditor.Networking.EventHandlers;

/// <summary>
/// Redirects calls to a specific event handler.
/// </summary>
internal class NetEventListenerAdapter : INetEventListener
{
    private readonly IPeerConnectedEventHandler _peerConnectedEventHandler;
    private readonly IPeerDisconnectedEventHandler _peerDisconnectedEventHandler;
    private readonly INetworkErrorEventHandler _networkErrorEventHandler;
    private readonly INetworkReceiveEventHandler _networkReceiveEventHandler;
    private readonly INetworkReceiveUnconnectedEventHandler _networkReceiveUnconnectedEventHandler;
    private readonly INetworkLatencyUpdateEventHandler _networkLatencyUpdateEventHandler;
    private readonly IConnectionRequestEventHandler _connectionRequestEventHandler;

    public NetEventListenerAdapter(
        IPeerConnectedEventHandler peerConnectedEventHandler,
        IPeerDisconnectedEventHandler peerDisconnectedEventHandler,
        INetworkErrorEventHandler networkErrorEventHandler,
        INetworkReceiveEventHandler networkReceiveEventHandler,
        INetworkReceiveUnconnectedEventHandler networkReceiveUnconnectedEventHandler,
        INetworkLatencyUpdateEventHandler networkLatencyUpdateEventHandler,
        IConnectionRequestEventHandler connectionRequestEventHandler)
    {
        _peerConnectedEventHandler = peerConnectedEventHandler;
        _peerDisconnectedEventHandler = peerDisconnectedEventHandler;
        _networkErrorEventHandler = networkErrorEventHandler;
        _networkReceiveEventHandler = networkReceiveEventHandler;
        _networkReceiveUnconnectedEventHandler = networkReceiveUnconnectedEventHandler;
        _networkLatencyUpdateEventHandler = networkLatencyUpdateEventHandler;
        _connectionRequestEventHandler = connectionRequestEventHandler;
    }

    void INetEventListener.OnPeerConnected(NetPeer peer)
    {
        _peerConnectedEventHandler.OnPeerConnected(peer);
    }

    void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _peerDisconnectedEventHandler.OnPeerDisconnected(peer, disconnectInfo);
    }

    void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        _networkErrorEventHandler.OnNetworkError(endPoint, socketError);
    }

    void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _networkReceiveEventHandler.OnNetworkReceive(peer, reader, deliveryMethod);
    }

    void INetEventListener.OnNetworkReceiveUnconnected(
        IPEndPoint remoteEndPoint,
        NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        _networkReceiveUnconnectedEventHandler.OnNetworkReceiveUnconnected(remoteEndPoint, reader, messageType);
    }

    void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        _networkLatencyUpdateEventHandler.OnNetworkLatencyUpdate(peer, latency);
    }

    void INetEventListener.OnConnectionRequest(ConnectionRequest request)
    {
        _connectionRequestEventHandler.OnConnectionRequest(request);
    }
}
