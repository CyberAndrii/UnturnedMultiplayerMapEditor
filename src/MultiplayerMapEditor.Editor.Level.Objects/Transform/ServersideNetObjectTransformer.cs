namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

internal sealed class ServersideNetObjectTransformer : INetObjectTransformer
{
    private readonly IServersideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public ServersideNetObjectTransformer(
        IServersideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public void TransformObject(NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale)
    {
        if (NetIdRegistry.GetTransform(netId, null) == null)
        {
            return;
        }

        _levelObjectsManager.TransformObject(
            netId,
            fromPosition,
            fromRotation,
            fromScale,
            toPosition,
            toRotation,
            toScale
        );
        SendTellTransformObject(netId, fromPosition, fromRotation, fromScale, toPosition, toRotation, toScale);
    }

    private void SendTellTransformObject(NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale)
    {
        _netPacketProcessor.Send(
            _netManager.NetManager,
            new TellTransformObjectPacket
            {
                NetId = netId,
                FromPosition = fromPosition,
                FromRotation = fromRotation,
                FromScale = fromScale,
                ToPosition = toPosition,
                ToRotation = toRotation,
                ToScale = toScale
            },
            DeliveryMethod.ReliableUnordered
        );
    }
}
