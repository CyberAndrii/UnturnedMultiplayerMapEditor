namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

/// <summary>
/// Alternative of the <see cref="SDG.Unturned.ReunObjectTransform"/> class with networking support.
/// </summary>
internal sealed class NetReunObjectTransform : INetReun
{
    private readonly Vector3 _fromPosition;
    private readonly Quaternion _fromRotation;
    private readonly Vector3 _fromScale;
    private readonly Vector3 _toPosition;
    private readonly Quaternion _toRotation;
    private readonly Vector3 _toScale;
    private readonly INetObjectTransformer _netObjectTransformer;

    private readonly ReunStateLock _state = new(
        currentState: ReunState.Undo,
        canEnterFunc: (currentState, newState) => newState switch
        {
            ReunState.Redo => currentState is ReunState.Undo,
            ReunState.Waiting => false,
            ReunState.Undo => currentState is ReunState.Redo,
            _ => false
        }
    );

    public NetReunObjectTransform(
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale,
        UnityEngine.Transform? transform,
        int step,
        INetObjectTransformer netObjectTransformer)
    {
        _fromPosition = fromPosition;
        _fromRotation = fromRotation;
        _fromScale = fromScale;
        _toPosition = toPosition;
        _toRotation = toRotation;
        _toScale = toScale;
        _netObjectTransformer = netObjectTransformer;
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

        _netObjectTransformer.TransformObject(
            NetId,
            _fromPosition,
            _fromRotation,
            _fromScale,
            _toPosition,
            _toRotation,
            _toScale
        );

        return null;
    }

    public void undo()
    {
        if (!_state.TryEnter(ReunState.Undo))
        {
            return;
        }

        _netObjectTransformer.TransformObject(NetId,
            _toPosition,
            _toRotation,
            _toScale,
            _fromPosition,
            _fromRotation,
            _fromScale
        );
    }

    public static NetReunObjectTransform From(ReunObjectTransform reun, INetObjectTransformer netObjectTransformer)
    {
        return new NetReunObjectTransform(
            reun.GetFromPosition(),
            reun.GetFromRotation(),
            reun.GetFromScale(),
            reun.GetToPosition(),
            reun.GetToRotation(),
            reun.GetToScale(),
            reun.GetModel(),
            reun.step,
            netObjectTransformer
        );
    }
}
