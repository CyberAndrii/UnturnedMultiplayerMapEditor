using MultiplayerMapEditor.Editor.Level.Objects.Remove;

namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

/// <summary>
/// Alternative of the <see cref="SDG.Unturned.ReunObjectAdd"/> class with networking support.
/// </summary>
internal sealed class NetReunObjectAdd : IReun
{
    private readonly Vector3 _position;
    private readonly Quaternion _rotation;
    private readonly Vector3 _scale;
    private readonly ObjectAsset? _objectAsset;
    private readonly ItemAsset? _itemAsset;
    private readonly INetObjectCreator _netObjectCreator;
    private readonly INetObjectRemover _netObjectRemover;

    internal NetId NetId;

    private readonly ReunStateLock _state = new(
        currentState: ReunState.Undo,
        canEnterFunc: (currentState, newState) => newState switch
        {
            ReunState.Redo => currentState is ReunState.Waiting,
            ReunState.Waiting => currentState is ReunState.Undo,
            ReunState.Undo => currentState is ReunState.Redo,
            _ => false
        }
    );

    public NetReunObjectAdd(
        int step,
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ObjectAsset? objectAsset,
        ItemAsset? itemAsset,
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
    }

    public int step { get; }

    public Transform? redo()
    {
        if (!_state.TryEnter(ReunState.Waiting))
        {
            return null;
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

        return null;
    }

    private void OnCreated(Transform transform, NetId netId)
    {
        if (!_state.TryEnter(ReunState.Redo))
        {
            return;
        }

        NetId = netId;

        var reuns = LevelObjectsReflection.GetReuns();
        var indexOfThis = Array.IndexOf(reuns, this);

        if (indexOfThis < 1)
        {
            return;
        }

        if (reuns[indexOfThis - 1] is NetReunObjectRemove reunRemove)
        {
            reunRemove.NetId = netId;
        }
    }

    public void undo()
    {
        if (!_state.TryEnter(ReunState.Undo))
        {
            return;
        }

        _netObjectRemover.Remove(NetId);
    }

    public static NetReunObjectAdd From(
        ReunObjectAdd reun,
        INetObjectCreator netObjectCreator,
        INetObjectRemover netObjectRemover)
    {
        return new NetReunObjectAdd(
            reun.step,
            reun.GetPosition(),
            reun.GetRotation(),
            reun.GetScale(),
            reun.GetObjectAsset(),
            reun.GetItemAsset(),
            netObjectCreator,
            netObjectRemover
        );
    }
}
