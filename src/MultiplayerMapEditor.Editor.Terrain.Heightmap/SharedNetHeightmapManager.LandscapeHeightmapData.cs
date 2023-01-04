namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

partial class SharedNetHeightmapManager
{
    /// <summary>
    /// Stores additional data of a tile used internally by the <see cref="SharedNetHeightmapManager"/> class.
    /// </summary>
    /// <param name="TileCoord">
    /// Coordinates of a tile this <see cref="LandscapeHeightmapData"/> instance contains data for.
    /// </param>
    protected sealed record LandscapeHeightmapData(LandscapeCoord TileCoord)
    {
        private Dirtyable<float[,]> _deferredApplyDifference;

        /// <summary>
        /// Pending changes that will be applied to <see cref="LandscapeTile.heightmap"/> on next tick.
        /// </summary>
        public ref Dirtyable<float[,]> DeferredApplyDifference => ref _deferredApplyDifference;

        /// <summary>
        /// Changes made by the current transaction scope.
        /// </summary>
        public required float[,]? CurrentTransactionDifference { get; set; }

        public bool NeedsHeightmapSync { get; set; }
    }
}
