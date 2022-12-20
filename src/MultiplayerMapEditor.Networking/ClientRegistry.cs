using Microsoft.Extensions.Options;

namespace MultiplayerMapEditor.Networking;

internal sealed class ClientRegistry : IClientRegistry
{
    private readonly INetManager _netManager;
    private readonly ServerOptions _serverOptions;
    private readonly ILogger<ClientRegistry> _logger;
    private readonly List<ConnectingPlayer> _connectingPlayers;
    private readonly ThreadLocal<NetDataWriter> _threadLocalWriter;

    public ClientRegistry(
        INetManager netManager,
        IOptions<ServerOptions> serverOptions,
        ILogger<ClientRegistry> logger)
    {
        if (netManager is not IServersideNetManager)
        {
            throw new ArgumentException("This class must only be used serverside.", nameof(ClientRegistry));
        }

        _netManager = netManager;
        _serverOptions = serverOptions.Value;
        _logger = logger;
        _connectingPlayers = new List<ConnectingPlayer>();
        _threadLocalWriter = new ThreadLocal<NetDataWriter>(() => new NetDataWriter());
    }

    public IReadOnlyList<ConnectingPlayer> ConnectingPlayers => _connectingPlayers;

    public void AcceptOrReject(ConnectionRequest request)
    {
        if (_connectingPlayers.Count >= _serverOptions.MaxQueueSize)
        {
            Reject(request, "Queue is full");
            return;
        }

        ConnectionRequestData data;

        try
        {
            data = request.Data.Get<ConnectionRequestData>();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex,
                "An exception has been thrown while deserializing connection data from {EndPoint}. " +
                "Most likely this is caused by a invalid packet sent by the peer and can be safely ignored",
                request.RemoteEndPoint
            );

            Reject(request,
                "Failed to deserialize connection packet. " +
                "Ensure that you have the same version of the module as the server"
            );
            return;
        }

        if (!IsConnectionRequestDataValid(data, out var rejectMessage))
        {
            Reject(request, rejectMessage);
            return;
        }


        Accept(request, data);
    }

    private bool IsConnectionRequestDataValid(ConnectionRequestData data, out string rejectMessage)
    {
        if (data.SteamId == 0)
        {
            rejectMessage = "Invalid SteamId";
            return false;
        }

        if (_connectingPlayers.Any(p => p.ConnectionRequestData.SteamId == data.SteamId))
        {
            rejectMessage = "Already connected";
            return false;
        }

        data.Username = data.Username.Trim();

        if (data.Username.Length is < 3 or > 25)
        {
            rejectMessage = "Username is too short or too long";
            return false;
        }

        rejectMessage = "";
        return true;
    }

    private void Reject(ConnectionRequest request, string rejectMessage)
    {
        var rejectReason = new RejectReason { Message = rejectMessage };
        Reject(request, rejectReason);
    }

    private void Reject(ConnectionRequest request, RejectReason rejectReason)
    {
        var writer = _threadLocalWriter.Value;
        writer.Reset();
        writer.Put(rejectReason);
        request.Reject(writer);

        _logger.LogInformation(
            "Rejected connection request from {EndPoint}: {Reason}",
            request.RemoteEndPoint,
            rejectReason.Message
        );
    }

    private ConnectingPlayer Accept(ConnectionRequest request, ConnectionRequestData data)
    {
        var peer = request.Accept();

        var connectingPlayer = new ConnectingPlayer
        {
            NetPeer = peer, //
            ConnectionRequestData = data,
        };

        _connectingPlayers.Add(connectingPlayer);

        return connectingPlayer;
    }

    public void RemoveDisconnected(NetPeer peer)
    {
        _connectingPlayers.RemoveAll(player => player.NetPeer == peer);
    }
}
