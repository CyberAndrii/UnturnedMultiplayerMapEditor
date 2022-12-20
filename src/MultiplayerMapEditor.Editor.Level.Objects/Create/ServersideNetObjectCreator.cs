namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal sealed class ServersideNetObjectCreator : IServersideNetObjectCreator
{
    private readonly IServersideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public ServersideNetObjectCreator(
        IServersideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public void CreateObject(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
        Action<Transform, NetId>? createdCallback,
        NetPeer? requestedBy,
        Guid? correlationId)
    {
        if (objectAsset != null)
        {
            var @object = _levelObjectsManager.Create(
                position,
                rotation,
                scale,
                objectAsset.id,
                objectAsset.GUID,
                ELevelObjectPlacementOrigin.MANUAL,
                instanceId: null,
                netId: NetIdRegistry.Claim(),
                AssetReference<MaterialPaletteAsset>.invalid,
                materialIndexOverride: null,
                select: requestedBy == null
            );

            if (@object == null)
            {
                return;
            }

            createdCallback?.Invoke(@object.Object.transform, @object.NetId);

            SendTellCreateObject(
                position,
                rotation,
                scale,
                objectAsset,
                itemAsset: null,
                requestedBy,
                correlationId,
                @object.Object.instanceID,
                @object.NetId,
                @object.Object.GetCustomMaterialOverrideGuid(),
                @object.Object.GetMaterialIndexOverride()
            );
        }
        else if (itemAsset != null)
        {
            var buildableObject = _levelObjectsManager.CreateBuildable(
                position,
                rotation,
                itemAsset.id,
                netId: NetIdRegistry.Claim(),
                select: requestedBy == null
            );

            if (buildableObject == null)
            {
                return;
            }

            createdCallback?.Invoke(buildableObject.BuildableObject!.transform, buildableObject.NetId);

            SendTellCreateObject(
                position,
                rotation,
                scale,
                objectAsset: null,
                itemAsset,
                requestedBy,
                correlationId,
                instanceId: 0,
                buildableObject.NetId,
                materialOverride: Guid.Empty,
                materialIndexOverride: null
            );
        }
    }

    private void SendTellCreateObject(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
        NetPeer? requestedBy,
        Guid? correlationId,
        uint instanceId,
        NetId netId,
        Guid materialOverride,
        int? materialIndexOverride)
    {
        var tellPacket = new TellCreateObjectPacket
        {
            Position = position,
            Rotation = rotation,
            Scale = scale,
            ObjectAsset = objectAsset?.GUID ?? Guid.Empty,
            ItemAsset = itemAsset?.GUID ?? Guid.Empty,
            InstanceId = instanceId,
            NetId = netId,
            MaterialOverride = materialOverride,
            MaterialIndexOverride = materialIndexOverride,
        };

        foreach (var peer in _netManager.NetManager.ConnectedPeerList)
        {
            tellPacket.Select = requestedBy != null && peer.Id == requestedBy.Id;
            tellPacket.CorrelationId = peer.Id == requestedBy?.Id ? correlationId : null;

            _netPacketProcessor.Send(peer, tellPacket, DeliveryMethod.ReliableUnordered);
        }
    }
}
