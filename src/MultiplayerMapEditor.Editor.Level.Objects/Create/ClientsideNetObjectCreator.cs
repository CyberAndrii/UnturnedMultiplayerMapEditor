namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal sealed class ClientsideNetObjectCreator : IClientsideNetObjectCreator
{
    private readonly IClientsideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;
    private readonly Dictionary<Guid, Action<UnityEngine.Transform, NetId>> _createdCallbacks = new();

    public ClientsideNetObjectCreator(
        IClientsideNetManager netManager,
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
        Action<UnityEngine.Transform, NetId>? createdCallback)
    {
        var correlationId = Guid.NewGuid();

        if ((objectAsset == null && itemAsset == null) ||
            (objectAsset?.GUID == Guid.Empty && itemAsset?.GUID == Guid.Empty))
        {
            throw new ArgumentNullException(
                $"One of {nameof(objectAsset)} and {nameof(itemAsset)} parameters must be set"
            );
        }

        _netPacketProcessor.Send(
            _netManager.ServerPeer!,
            new AskCreateObjectPacket
            {
                Position = position,
                Rotation = rotation,
                Scale = scale,
                ObjectAsset = objectAsset?.GUID ?? Guid.Empty,
                ItemAsset = itemAsset?.GUID ?? Guid.Empty,
                CorrelationId = correlationId,
            },
            DeliveryMethod.ReliableUnordered
        );

        if (createdCallback != null)
        {
            _createdCallbacks.Add(correlationId, createdCallback);
        }
    }


    public void NotifyCreated(Guid correlationId, UnityEngine.Transform transform, NetId netId)
    {
        if (!_createdCallbacks.TryGetValue(correlationId, out var callback))
        {
            return;
        }


        callback(transform, netId);
    }
}
