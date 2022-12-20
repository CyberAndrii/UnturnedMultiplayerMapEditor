namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

internal sealed class ServersideNetObjectRemover : INetObjectRemover
{
    private readonly IServersideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public ServersideNetObjectRemover(
        IServersideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public void Remove(NetId netId)
    {
        if (NetIdRegistry.GetTransform(netId, null) == null)
        {
            return;
        }

        _levelObjectsManager.Remove(netId);
        SendTellRemoveObject(netId);
    }

    private void SendTellRemoveObject(NetId netId)
    {
        _netPacketProcessor.Send(
            _netManager.NetManager,
            new TellRemoveObjectPacket { NetId = netId },
            DeliveryMethod.ReliableUnordered
        );
    }
}
