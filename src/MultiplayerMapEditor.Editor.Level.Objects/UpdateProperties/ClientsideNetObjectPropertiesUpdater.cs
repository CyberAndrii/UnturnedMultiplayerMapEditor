namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal sealed class ClientsideNetObjectPropertiesUpdater : INetObjectPropertiesUpdater
{
    private readonly IClientsideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;

    public ClientsideNetObjectPropertiesUpdater(
        IClientsideNetManager netManager,
        INetPacketProcessor netPacketProcessor)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
    }

    public void UpdateObjectProperties(NetId netId, Guid materialOverride, int materialIndexOverride)
    {
        SendAskUpdateObjectProperties(netId, materialOverride, materialIndexOverride);
    }

    private void SendAskUpdateObjectProperties(NetId netId, Guid materialOverride, int materialIndexOverride)
    {
        _netPacketProcessor.Send(
            _netManager.ServerPeer!,
            new AskUpdateObjectPropertiesPacket
            {
                NetId = netId, //
                MaterialOverride = materialOverride,
                MaterialIndexOverride = materialIndexOverride
            },
            DeliveryMethod.ReliableUnordered
        );
    }
}
