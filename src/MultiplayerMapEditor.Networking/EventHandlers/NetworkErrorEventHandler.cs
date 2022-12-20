using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class ClientsideNetworkErrorEventHandler : INetworkErrorEventHandler
{
    private readonly ClientOptions _clientOptions;

    public ClientsideNetworkErrorEventHandler(IOptions<ClientOptions> clientOptions)
    {
        _clientOptions = clientOptions.Value;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        if (socketError == SocketError.Success)
        {
            return;
        }

        _clientOptions.StopAsyncFunc("Socket error: " + socketError).Forget();
    }
}

internal sealed class ServersideNetworkErrorEventHandler : INetworkErrorEventHandler
{
    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        // todo: handle network error serverside
    }
}
