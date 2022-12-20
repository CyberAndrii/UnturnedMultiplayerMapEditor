using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal sealed class AskCreateObjectPacket
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Guid ObjectAsset { get; set; }
    public Guid ItemAsset { get; set; }
    public Guid? CorrelationId { get; set; }
}

internal sealed class AskCreateObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly IServersideNetObjectCreator _netObjectCreator;

    public AskCreateObjectRPC(
        INetPacketProcessor netPacketProcessor,
        IServersideNetObjectCreator netObjectCreator)
    {
        _netPacketProcessor = netPacketProcessor;
        _netObjectCreator = netObjectCreator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<AskCreateObjectPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<AskCreateObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(AskCreateObjectPacket packet, NetPeer peer)
    {
        _netObjectCreator.CreateObject(
            packet.Position,
            packet.Rotation,
            packet.Scale,
            packet.ObjectAsset == Guid.Empty ? null : Assets.find<ObjectAsset>(packet.ObjectAsset),
            packet.ItemAsset == Guid.Empty ? null : Assets.find<ItemAsset>(packet.ItemAsset),
            createdCallback: null,
            requestedBy: peer,
            packet.CorrelationId
        );
    }
}
