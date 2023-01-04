namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// Synchronizes landscape's heightmap over the network.
/// </summary>
internal interface INetHeightmapManager
{
    /// <summary>
    /// Decorates the <see cref="Landscape.writeHeightmap"/> method.
    /// Adds custom logic to the <paramref name="callback"/>.
    /// </summary>
    /// <param name="worldBounds"></param>
    /// <param name="callback"></param>
    void OnWriteHeightmap(Bounds worldBounds, ref Landscape.LandscapeWriteHeightmapHandler callback);

    /// <summary>
    /// Registers height <paramref name="differences"/>.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="differences"></param>
    void ApplyDifference(LandscapeTile tile, float[,] differences);

    /// <summary>
    /// Unregisters height <paramref name="differences"/>.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="differences"></param>
    void UnapplyDifference(LandscapeTile tile, float[,] differences);

    /// <summary>
    /// Applies height differences directly to the <paramref name="tile"/> without sending them to the server or to clients.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="differences"></param>
    void ApplyDifferenceLocal(LandscapeTile tile, float[,] differences);

    /// <summary>
    /// Begins recording height differences.
    /// </summary>
    /// <param name="tileCoord"></param>
    /// <param name="differences"></param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a transaction was already began and not yet ended at the specified <paramref name="tileCoord"/>.
    /// </exception>
    void BeginTransaction(LandscapeCoord tileCoord);

    /// <summary>
    /// Gets the height differences made in the current transaction.
    /// <see cref="BeginTransaction"/> must be called before call to this method.
    /// Important: caller is the one who is responsible of returning the heightmap into the pool.
    /// </summary>
    /// <param name="tileCoord"></param>
    /// <returns>Height differences made in the current transaction.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a transaction was not began or already ended at the specified <paramref name="tileCoord"/>.
    /// </exception>
    float[,] EndTransaction(LandscapeCoord tileCoord);

    void EndAllTransactions();
}
