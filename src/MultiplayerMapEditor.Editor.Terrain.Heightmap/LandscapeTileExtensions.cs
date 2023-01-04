using SDG.Framework.Devkit;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

internal static class LandscapeTileExtensions
{
    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/d2c09ed739163b4aa6eb2af5d7aa6119d56300d4/Assembly-CSharp/SDG.Framework.Landscapes/Landscape.cs#L426..L474
    public static void UpdateTilesBorderHeights(LandscapeBounds landscapeBounds)
    {
        for (var x = landscapeBounds.min.x; x <= landscapeBounds.max.x; ++x)
        {
            for (var y = landscapeBounds.min.y; y <= landscapeBounds.max.y; ++y)
            {
                var tile = Landscape.getTile(new LandscapeCoord(x, y));

                if (tile == null)
                {
                    continue;
                }

                UpdateTileBorderHeights(tile, landscapeBounds);
            }
        }
    }

    /// <summary>
    /// Sets East and North border heights to neighbour tiles' heights.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="landscapeBounds"></param>
    public static void UpdateTileBorderHeights(this LandscapeTile tile, LandscapeBounds landscapeBounds)
    {
        var x = tile.coord.x;
        var y = tile.coord.y;

        // Update east border's height
        if (x < landscapeBounds.max.x)
        {
            var eastTile = Landscape.getTile(new LandscapeCoord(x + 1, y));

            if (eastTile != null)
            {
                for (var index = 0; index <= Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE; ++index)
                {
                    tile.heightmap[
                        index,
                        Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE
                    ] = eastTile.heightmap[index, 0];
                }
            }
        }

        // Update north border's height
        if (y < landscapeBounds.max.y)
        {
            var northTile = Landscape.getTile(new LandscapeCoord(x, y + 1));

            if (northTile != null)
            {
                for (var index = 0; index <= Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE; ++index)
                {
                    tile.heightmap[
                        Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE,
                        index
                    ] = northTile.heightmap[0, index];
                }
            }
        }

        // Update north east border's height (single vertex)
        if (x < landscapeBounds.max.x && y < landscapeBounds.max.y)
        {
            var northEastTile = Landscape.getTile(new LandscapeCoord(x + 1, y + 1));

            if (northEastTile != null)
            {
                tile.heightmap[
                    Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE,
                    Landscape.HEIGHTMAP_RESOLUTION_MINUS_ONE
                ] = northEastTile.heightmap[0, 0];
            }
        }

        // Set terrain heights (without rebuilding LODs - for performance)
        tile.SetHeightsDelayLOD();

        // Mark level has unsaved changes
        LevelHierarchy.MarkDirty();
    }
}
