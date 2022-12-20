using System.Net;

namespace MultiplayerMapEditor.Networking;

internal sealed class ClientsideNetManager : NetManagerBase, IClientsideNetManager
{
    private readonly ILogger<ClientsideNetManager> _logger;
    private readonly ThreadLocal<NetDataWriter> _threadLocalWriter;

    public ClientsideNetManager(
        ILogger<ClientsideNetManager> logger,
        Lazy<INetEventListener> netEventListenerLazy) : base(netEventListenerLazy, logger)
    {
        _logger = logger;
        _threadLocalWriter = new ThreadLocal<NetDataWriter>(() => new NetDataWriter());
    }

    public bool IsConnected => LowLevelManager.FirstPeer?.ConnectionState == ConnectionState.Connected;

    public NetPeer? ServerPeer => LowLevelManager.FirstPeer;

    public void Connect(IPEndPoint ipEndPoint, ConnectionRequestData data)
    {
        AssertIsNotInitialized();

        IsInitialized = true;

        _logger.LogInformation("Connecting to {EndPoint}.", ipEndPoint);

        var writer = _threadLocalWriter.Value;
        writer.Reset();
        writer.Put(data);

        LowLevelManager.Start();
        LowLevelManager.Connect(ipEndPoint, writer);

        PollEventsAsync().Forget();
    }
}
