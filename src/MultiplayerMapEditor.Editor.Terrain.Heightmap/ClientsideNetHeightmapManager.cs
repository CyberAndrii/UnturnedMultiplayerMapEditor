using MultiplayerMapEditor.Editor.Terrain.Heightmap.Update;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// <inheritdoc cref="INetHeightmapManager"/>
/// Runs clientside only.
/// </summary>
internal sealed class ClientsideNetHeightmapManager : SharedNetHeightmapManager
{
    private readonly IClientsideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;

    public ClientsideNetHeightmapManager(
        IClientsideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILogger<ClientsideNetHeightmapManager> logger) : base(logger)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
    }

    /// <summary>
    /// Sends all pending changes to the server.
    /// </summary>
    /// <param name="heightmapDataByCoord"></param>
    protected override void SendDifferences(
        IReadOnlyDictionary<LandscapeCoord, LandscapeHeightmapData> heightmapDataByCoord)
    {
        foreach (var kvp in heightmapDataByCoord)
        {
            if (!kvp.Value.DeferredApplyDifference.IsDirty)
            {
                continue;
            }

            var packet = new AskUpdateHeightmapPacket
            {
                Coord = kvp.Key, //
                DifferenceHeightmap = kvp.Value.DeferredApplyDifference
            };

            _netPacketProcessor.SendNetSerializable(_netManager.ServerPeer!, packet, DeliveryMethod.ReliableUnordered);
        }
    }
}
