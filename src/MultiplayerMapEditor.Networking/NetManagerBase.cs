namespace MultiplayerMapEditor.Networking;

internal abstract class NetManagerBase : INetManager, IDisposable
{
    private readonly Lazy<INetEventListener> _netEventListenerLazy;
    private readonly ILogger<NetManagerBase> _logger;
    private LiteNetLib.NetManager? _lowLevelManager; // lazily initialized
    protected readonly CancellationTokenSource _cts;

    public NetManagerBase(
        Lazy<INetEventListener> netEventListenerLazy,
        ILogger<NetManagerBase> logger)
    {
        _netEventListenerLazy = netEventListenerLazy;
        _logger = logger;
        _cts = new CancellationTokenSource();
    }

    protected LiteNetLib.NetManager LowLevelManager
        => _lowLevelManager ??= new LiteNetLib.NetManager(_netEventListenerLazy.Value);

    protected bool IsInitialized { get; set; }

    public bool IsRunning => LowLevelManager.IsRunning;

    public NetManager NetManager => LowLevelManager;

    protected void AssertIsNotInitialized()
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Already initialized.");
        }
    }

    protected async UniTaskVoid PollEventsAsync()
    {
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate())
        {
            try
            {
                LowLevelManager.PollEvents();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has been thrown in the PollEvents loop");
            }

            if (_cts.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public virtual void Dispose()
    {
        _lowLevelManager?.Stop(sendDisconnectMessages: true);
        _cts.Cancel();
    }
}
