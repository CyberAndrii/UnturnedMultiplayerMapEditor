namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// Provides additional methods for the <see cref="SDG.Framework.Landscapes.LandscapeHeightmapCopyPool"/> class.
/// </summary>
internal static class LandscapeHeightmapCopyPoolEx
{
    /// <summary>
    /// Claims an array from the pool and fills it with zeros.
    /// </summary>
    /// <returns>Returns cleared array from the pool.</returns>
    public static float[,] ClaimClean()
    {
        var array = LandscapeHeightmapCopyPool.claim();
        Array.Clear(array, 0, array.Length);
        return array;
    }
}
