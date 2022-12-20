using System.Net;
using System.Net.Sockets;

namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface INetworkErrorEventHandler
{
    void OnNetworkError(IPEndPoint endPoint, SocketError socketError);
}
