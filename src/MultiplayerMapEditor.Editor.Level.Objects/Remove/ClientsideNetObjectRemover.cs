namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

internal sealed class ClientsideNetObjectRemover : INetObjectRemover
{
    private readonly IClientsideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;

    public ClientsideNetObjectRemover(
        IClientsideNetManager netManager,
        INetPacketProcessor netPacketProcessor)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
    }

    public void Remove(NetId netId)
    {
        // todo: pass path?
        SendAskRemoveObject(netId);
    }

    private void SendAskRemoveObject(NetId netId)
    {
        _netPacketProcessor.Send(
            _netManager.ServerPeer!,
            new AskRemoveObjectPacket { NetId = netId },
            DeliveryMethod.ReliableUnordered
        );
    }
}
