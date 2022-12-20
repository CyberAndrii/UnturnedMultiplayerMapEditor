using System.Net;

namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface INetworkReceiveUnconnectedEventHandler
{
    void OnNetworkReceiveUnconnected(
        IPEndPoint remoteEndPoint,
        NetPacketReader reader,
        UnconnectedMessageType messageType
    );
}
