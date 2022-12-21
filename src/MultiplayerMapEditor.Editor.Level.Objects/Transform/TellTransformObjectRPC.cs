using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

internal sealed class TellTransformObjectPacket
{
    public NetId NetId { get; set; }
    public Vector3 FromPosition { get; set; }
    public Quaternion FromRotation { get; set; }
    public Vector3 FromScale { get; set; }
    public Vector3 ToPosition { get; set; }
    public Quaternion ToRotation { get; set; }
    public Vector3 ToScale { get; set; }
}

internal sealed class TellTransformObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public TellTransformObjectRPC(
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<TellTransformObjectPacket>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<TellTransformObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(TellTransformObjectPacket packet)
    {
        _levelObjectsManager.TransformObject(
            packet.NetId,
            packet.FromPosition,
            packet.FromRotation,
            packet.FromScale,
            packet.ToPosition,
            packet.ToRotation,
            packet.ToScale
        );
    }
}
