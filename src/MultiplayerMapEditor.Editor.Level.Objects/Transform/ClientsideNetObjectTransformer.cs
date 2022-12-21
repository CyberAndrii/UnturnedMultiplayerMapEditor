namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

internal sealed class ClientsideNetObjectTransformer : INetObjectTransformer
{
    private readonly IClientsideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;

    public ClientsideNetObjectTransformer(
        IClientsideNetManager netManager,
        INetPacketProcessor netPacketProcessor)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
    }

    public void TransformObject(
        NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale)
    {
        SendAskTransformObject(netId, fromPosition, fromRotation, fromScale, toPosition, toRotation, toScale);
    }

    private void SendAskTransformObject(
        NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale)
    {
        _netPacketProcessor.Send(
            _netManager.ServerPeer!,
            new AskTransformObjectPacket
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
