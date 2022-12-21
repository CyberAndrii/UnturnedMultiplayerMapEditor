using MultiplayerMapEditor.Editor.Level.Objects.Create;

namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

/// <summary>
/// Alternative of the <see cref="SDG.Unturned.ReunObjectRemove"/> class with networking support.
/// </summary>
internal sealed class NetReunObjectRemove : INetReun
{
    private readonly Vector3 _position;
    private readonly Quaternion _rotation;
    private readonly Vector3 _scale;
    private readonly ObjectAsset? _objectAsset;
    private readonly ItemAsset? _itemAsset;
    private readonly INetObjectCreator _netObjectCreator;
    private readonly INetObjectRemover _netObjectRemover;

    private readonly ReunStateLock _state = new(
        currentState: ReunState.Undo,
        canEnterFunc: (currentState, newState) => newState switch
        {
            ReunState.Redo => currentState is ReunState.Undo,
            ReunState.Waiting => currentState is ReunState.Redo,
            ReunState.Undo => currentState is ReunState.Waiting,
            _ => false
        }
    );

    public NetReunObjectRemove(
        int step,
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
        UnityEngine.Transform? transform,
        INetObjectCreator netObjectCreator,
        INetObjectRemover netObjectRemover)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
        _objectAsset = objectAsset;
        _itemAsset = itemAsset;
        _netObjectCreator = netObjectCreator;
        _netObjectRemover = netObjectRemover;
        this.step = step;

        if (!NetIdRegistry.GetTransformNetId(transform, out var netId, out var path))
        {
            throw new ArgumentException("NetId was not found", nameof(transform));
        }

        NetId = netId;
    }

    public NetId NetId { get; set; }

    public int step { get; }

    public UnityEngine.Transform? redo()
    {
        if (!_state.TryEnter(ReunState.Redo))
        {
            return null;
        }

        _netObjectRemover.Remove(NetId);
        return null;
    }

    public void undo()
    {
        if (!_state.TryEnter(ReunState.Waiting))
        {
            return;
        }

        if (_netObjectCreator is IClientsideNetObjectCreator clientsideNetObjectCreator)
        {
            clientsideNetObjectCreator.CreateObject(
                _position,
                _rotation,
                _scale,
                _objectAsset,
                _itemAsset,
                createdCallback: OnCreated
            );
        }
        else if (_netObjectCreator is IServersideNetObjectCreator serversideNetManager)
        {
            serversideNetManager.CreateObject(
                _position,
                _rotation,
                _scale,
                _objectAsset,
                _itemAsset,
                createdCallback: OnCreated,
                requestedBy: null,
                correlationId: null
            );
        }
    }

    private void OnCreated(UnityEngine.Transform transform, NetId netId)
    {
        if (!_state.TryEnter(ReunState.Undo))
        {
            return;
        }

        LevelObjectsReunLinker.ReplaceNetId(oldNetId: NetId, newNetId: netId);
    }

    public static NetReunObjectRemove From(
        ReunObjectRemove reun,
        INetObjectCreator netObjectCreator,
        INetObjectRemover netObjectRemover)
    {
        return new NetReunObjectRemove(
            reun.step,
            reun.GetPosition(),
            reun.GetRotation(),
            reun.GetScale(),
            reun.GetObjectAsset(),
            reun.GetItemAsset(),
            reun.GetModel(),
            netObjectCreator,
            netObjectRemover
        );
    }
}
