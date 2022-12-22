namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal sealed class ServersideNetObjectPropertiesUpdater : INetObjectPropertiesUpdater
{
    private readonly IServersideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public ServersideNetObjectPropertiesUpdater(
        IServersideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public void UpdateObjectProperties(NetId netId, Guid materialOverride, int materialIndexOverride)
    {
        if (NetIdRegistry.GetTransform(netId, null) == null)
        {
            return;
        }

        _levelObjectsManager.UpdateObjectProperties(netId, materialOverride, materialIndexOverride);
        SendTellUpdateObjectProperties(netId, materialOverride, materialIndexOverride);
    }

    private void SendTellUpdateObjectProperties(NetId netId, Guid materialOverride, int materialIndexOverride)
    {
        _netPacketProcessor.Send(
            _netManager.NetManager,
            new TellUpdateObjectPropertiesPacket
            {
                NetId = netId, //
                MaterialOverride = materialOverride,
                MaterialIndexOverride = materialIndexOverride,
            },
            DeliveryMethod.ReliableUnordered
        );
    }
}
