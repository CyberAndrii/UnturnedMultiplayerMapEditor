namespace MultiplayerMapEditor.Networking.Abstractions;

public abstract class SharedOptions
{
    public virtual Func<string, UniTask> StopAsyncFunc { get; set; } =
        disconnectReason => throw new NotImplementedException();
}

public class ClientOptions : SharedOptions
{
}

public class ServerOptions : SharedOptions
{
    public virtual int Port { get; set; } = 27015;

    public virtual int MaxConnections { get; set; } = 12;

    public virtual int MaxQueueSize { get; set; } = 6;

    public virtual int MaxLatency { get; set; } = 1000;
}
