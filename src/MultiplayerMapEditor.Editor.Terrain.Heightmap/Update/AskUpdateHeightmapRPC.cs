namespace MultiplayerMapEditor.Editor.Terrain.Heightmap.Update;

internal sealed class AskUpdateHeightmapPacket : INetSerializable, IDisposable
{
    private bool _shouldReleaseIntoPool;

    public LandscapeCoord Coord { get; set; }
    public float[,]? DifferenceHeightmap { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        // Write landscape coordinates
        writer.Put(Coord.x);
        writer.Put(Coord.y);

        var heightsCount = 0;

        // Make space for heights count that will be written later
        var countPosition = writer.Length;
        writer.SetPosition(countPosition + sizeof(int));

        for (var x = 0; x < Landscape.HEIGHTMAP_RESOLUTION; x++)
        {
            for (var y = 0; y < Landscape.HEIGHTMAP_RESOLUTION; y++)
            {
                var difference = DifferenceHeightmap![x, y];

                if (difference == 0)
                {
                    continue;
                }

                // todo: this can be further optimized by writing x only once
                writer.Put(x);
                writer.Put(y);
                writer.Put(difference);
                heightsCount++;
            }
        }

        // Write the count
        var endPosition = writer.Length;
        writer.SetPosition(countPosition);
        writer.Put(heightsCount);
        writer.SetPosition(endPosition);
    }

    public void Deserialize(NetDataReader reader)
    {
        ReleaseIntoPoolIfAllowed();

        Coord = new LandscapeCoord(reader.GetInt(), reader.GetInt());
        var heightsCount = reader.GetInt();

        DifferenceHeightmap = LandscapeHeightmapCopyPoolEx.ClaimClean();
        _shouldReleaseIntoPool = true;

        for (var i = 0; i < heightsCount; i++)
        {
            var x = reader.GetInt();
            var y = reader.GetInt();
            var difference = reader.GetFloat();
            DifferenceHeightmap[x, y] = difference;
        }
    }

    ~AskUpdateHeightmapPacket()
    {
        ReleaseIntoPoolIfAllowed();
    }

    public void Dispose()
    {
        ReleaseIntoPoolIfAllowed();
    }

    private void ReleaseIntoPoolIfAllowed()
    {
        if (_shouldReleaseIntoPool && DifferenceHeightmap != null)
        {
            LandscapeHeightmapCopyPool.release(DifferenceHeightmap);
            DifferenceHeightmap = null!;
            _shouldReleaseIntoPool = false;
        }
    }
}

internal sealed class AskUpdateHeightmapRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly INetHeightmapManager _netHeightmapManager;

    public AskUpdateHeightmapRPC(
        INetPacketProcessor netPacketProcessor,
        INetHeightmapManager netHeightmapManager)
    {
        _netPacketProcessor = netPacketProcessor;
        _netHeightmapManager = netHeightmapManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeNetSerializable<AskUpdateHeightmapPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<AskUpdateHeightmapPacket>();
        return Task.CompletedTask;
    }

    private void Handle(AskUpdateHeightmapPacket packet, NetPeer peer)
    {
        try
        {
            var tile = Landscape.getTile(packet.Coord);

            if (tile == null)
            {
                return;
            }

            _netHeightmapManager.ApplyDifference(tile, packet.DifferenceHeightmap!);
        }
        finally
        {
            packet.Dispose();
        }
    }
}
