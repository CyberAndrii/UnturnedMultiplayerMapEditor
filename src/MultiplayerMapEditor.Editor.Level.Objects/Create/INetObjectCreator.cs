namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal interface INetObjectCreator
{
}

internal interface IClientsideNetObjectCreator : INetObjectCreator
{
    void CreateObject(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
        Action<UnityEngine.Transform, NetId>? createdCallback
    );

    void NotifyCreated(Guid correlationId, UnityEngine.Transform transform, NetId netId);
}

internal interface IServersideNetObjectCreator : INetObjectCreator
{
    void CreateObject(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
        Action<UnityEngine.Transform, NetId>? createdCallback,
        NetPeer? requestedBy,
        Guid? correlationId
    );
}
