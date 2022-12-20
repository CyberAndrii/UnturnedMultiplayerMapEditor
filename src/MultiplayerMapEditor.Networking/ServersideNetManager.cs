using Microsoft.Extensions.Options;

namespace MultiplayerMapEditor.Networking;

internal sealed class ServersideNetManager : NetManagerBase, IServersideNetManager
{
    private readonly ILogger<ServersideNetManager> _logger;
    private readonly IOptions<ServerOptions> _serverOptions;
    private readonly ThreadLocal<NetDataWriter> _threadLocalWriter;

    public ServersideNetManager(
        ILogger<ServersideNetManager> logger,
        IOptions<ServerOptions> serverOptions,
        Lazy<INetEventListener> netEventListenerLazy) : base(netEventListenerLazy, logger)
    {
        _logger = logger;
        _serverOptions = serverOptions;
        _threadLocalWriter = new ThreadLocal<NetDataWriter>(() => new NetDataWriter());
    }

    private ServerOptions ServerOptions => _serverOptions.Value;

    public void Host()
    {
        AssertIsNotInitialized();

        IsInitialized = true;

        LowLevelManager.Start(ServerOptions.Port);

        _logger.LogInformation("Hosting on port {Port}", ServerOptions.Port);

        PollEventsAsync().Forget();
    }

    public void Disconnect(NetPeer peer, Abstractions.Packets.DisconnectReason disconnectReason)
    {
        var writer = _threadLocalWriter.Value;
        writer.Reset();
        writer.Put(disconnectReason);
        peer.Disconnect(writer);
    }
}
