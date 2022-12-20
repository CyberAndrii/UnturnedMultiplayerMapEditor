using System.Net;
using Microsoft.Extensions.Options;
using DisconnectReason = LiteNetLib.DisconnectReason;

namespace MultiplayerMapEditor.Networking.EventHandlers;

internal static partial class PeerDisconnectedEventHandlerLoggerMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Peer {EndPoint} disconnected with reason: {Reason}")]
    public static partial void LogDisconnected(this ILogger logger, IPEndPoint endPoint, DisconnectReason reason);

    public static void LogDisconnected(this ILogger logger, NetPeer peer, DisconnectInfo disconnectInfo)
    {
        logger.LogDisconnected(peer.EndPoint, disconnectInfo.Reason);
    }
}

internal sealed class ClientsidePeerDisconnectedEventHandler : IPeerDisconnectedEventHandler
{
    private readonly ClientOptions _clientOptions;
    private readonly ILogger _logger;

    public ClientsidePeerDisconnectedEventHandler(
        IOptions<ClientOptions> clientOptions,
        ILogger<ClientsidePeerDisconnectedEventHandler> logger)
    {
        _clientOptions = clientOptions.Value;
        _logger = logger;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _logger.LogDisconnected(peer, disconnectInfo);

        HandlePeerDisconnected(peer, disconnectInfo);
    }

    private void HandlePeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (disconnectInfo.Reason == DisconnectReason.ConnectionRejected)
        {
            var rejectReason = ReadPacketSafe<RejectReason>(disconnectInfo.AdditionalData);
            var message = rejectReason.Message.Trim() == ""
                ? "Connection rejected"
                : "Connection rejected: " + rejectReason.Message;

            _clientOptions.StopAsyncFunc(message).Forget();
        }

        if (disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled)
        {
            var disconnectReason = ReadPacketSafe<Abstractions.Packets.DisconnectReason>(disconnectInfo.AdditionalData);
            var message = disconnectReason.Message.Trim() == ""
                ? "Disconnected"
                : "Disconnected: " + disconnectReason.Message;

            _clientOptions.StopAsyncFunc(message).Forget();
        }
        else
        {
            _clientOptions.StopAsyncFunc("Disconnected: " + disconnectInfo.Reason).Forget();
        }
    }

    private static T? ReadPacketSafe<T>(NetPacketReader netPacketReader) where T : INetSerializable, new()
    {
        try
        {
            return netPacketReader.Get<T>();
        }
        catch (IndexOutOfRangeException)
        {
            return default;
        }
    }
}

internal sealed class ServersidePeerDisconnectedEventHandler : IPeerDisconnectedEventHandler
{
    private readonly IClientRegistry _clientRegistry;
    private readonly ILogger _logger;

    public ServersidePeerDisconnectedEventHandler(
        IClientRegistry clientRegistry,
        ILogger<ServersidePeerDisconnectedEventHandler> logger)
    {
        _clientRegistry = clientRegistry;
        _logger = logger;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _logger.LogDisconnected(peer, disconnectInfo);

        _clientRegistry.RemoveDisconnected(peer);
    }
}
