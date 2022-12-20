using System.Net;

namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class NetworkReceiveUnconnectedEventHandler : INetworkReceiveUnconnectedEventHandler
{
    public void OnNetworkReceiveUnconnected(
        IPEndPoint remoteEndPoint,
        NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
    }
}
