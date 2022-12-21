using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

internal sealed class AskTransformObjectPacket
{
    public NetId NetId { get; set; }
    public Vector3 FromPosition { get; set; }
    public Quaternion FromRotation { get; set; }
    public Vector3 FromScale { get; set; }
    public Vector3 ToPosition { get; set; }
    public Quaternion ToRotation { get; set; }
    public Vector3 ToScale { get; set; }
}

internal sealed class AskTransformObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly INetObjectTransformer _netObjectTransformer;

    public AskTransformObjectRPC(
        INetPacketProcessor netPacketProcessor,
        INetObjectTransformer netObjectTransformer)
    {
        _netPacketProcessor = netPacketProcessor;
        _netObjectTransformer = netObjectTransformer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<AskTransformObjectPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<AskTransformObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(AskTransformObjectPacket packet, NetPeer peer)
    {
        _netObjectTransformer.TransformObject(
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
