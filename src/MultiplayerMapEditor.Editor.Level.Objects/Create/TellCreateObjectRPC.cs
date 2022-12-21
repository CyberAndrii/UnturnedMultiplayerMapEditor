using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal sealed class TellCreateObjectPacket
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Guid ObjectAsset { get; set; }
    public Guid ItemAsset { get; set; }
    public uint InstanceId { get; set; }
    public NetId NetId { get; set; }
    public Guid MaterialOverride { get; set; }
    public int? MaterialIndexOverride { get; set; }
    public bool Select { get; set; }
    public Guid? CorrelationId { get; set; }
}

internal sealed class TellCreateObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;
    private readonly IClientsideNetObjectCreator _netObjectCreator;

    public TellCreateObjectRPC(
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager,
        IClientsideNetObjectCreator netObjectCreator)
    {
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
        _netObjectCreator = netObjectCreator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<TellCreateObjectPacket>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<TellCreateObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(TellCreateObjectPacket packet)
    {
        if (packet.ObjectAsset != Guid.Empty)
        {
            CreateObject(packet);
        }
        else if (packet.ItemAsset != Guid.Empty)
        {
            CreateBuildableObject(packet);
        }
    }

    private void CreateObject(TellCreateObjectPacket packet)
    {
        var @object = _levelObjectsManager.Create(
            packet.Position,
            packet.Rotation,
            packet.Scale,
            id: 0,
            packet.ObjectAsset,
            ELevelObjectPlacementOrigin.MANUAL,
            packet.InstanceId,
            packet.NetId,
            new AssetReference<MaterialPaletteAsset>(packet.MaterialOverride),
            packet.MaterialIndexOverride,
            packet.Select
        );

        NotifyCreated(packet.CorrelationId, @object?.Object.transform, packet.NetId);
    }

    private void CreateBuildableObject(TellCreateObjectPacket packet)
    {
        var asset = Assets.find<ItemAsset>(packet.ItemAsset);

        if (asset == null)
        {
            return;
        }

        var buildableObject = _levelObjectsManager.CreateBuildable(
            packet.Position,
            packet.Rotation,
            asset.id,
            packet.NetId,
            packet.Select
        );

        NotifyCreated(packet.CorrelationId, buildableObject?.BuildableObject?.transform, packet.NetId);
    }

    private void NotifyCreated(Guid? correlationId, UnityEngine.Transform? transform, NetId netId)
    {
        if (correlationId == null || transform == null)
        {
            return;
        }

        _netObjectCreator.NotifyCreated(correlationId.Value, transform, netId);
    }
}
