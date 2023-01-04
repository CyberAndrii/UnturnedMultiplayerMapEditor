using MultiplayerMapEditor.Editor.Terrain.Heightmap.Update;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// <inheritdoc cref="INetHeightmapManager"/>
/// Runs serverside only.
/// </summary>
internal sealed class ServersideNetHeightmapManager : SharedNetHeightmapManager
{
    private readonly IServersideNetManager _netManager;
    private readonly INetPacketProcessor _netPacketProcessor;

    public ServersideNetHeightmapManager(
        IServersideNetManager netManager,
        INetPacketProcessor netPacketProcessor,
        ILogger<ServersideNetHeightmapManager> logger) : base(logger)
    {
        _netManager = netManager;
        _netPacketProcessor = netPacketProcessor;
    }

    /// <summary>
    /// Applies locally all changes and sends them to connected clients.
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

            var tile = Landscape.getTile(new LandscapeCoord(kvp.Key.x, kvp.Key.y));

            if (tile == null)
            {
                return;
            }

            ApplyDifferenceLocal(tile, kvp.Value.DeferredApplyDifference);

            var packet = new TellUpdateHeightmapPacket
            {
                Coord = tile.coord, //
                DifferenceHeightmap = kvp.Value.DeferredApplyDifference
            };

            _netPacketProcessor.SendNetSerializable(_netManager.NetManager, packet, DeliveryMethod.ReliableUnordered);
        }
    }
}
