using System.Runtime.CompilerServices;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap;

/// <summary>
/// <inheritdoc cref="INetHeightmapManager"/>
/// Base class that runs both clientside and serverside.
/// </summary>
internal abstract partial class SharedNetHeightmapManager : INetHeightmapManager, IDisposable
{
    private readonly ILogger<SharedNetHeightmapManager> _logger;

    /// <summary>
    /// Provides cancellation tokens for asynchronous operations used by the current instance of the class.
    /// </summary>
    private readonly CancellationTokenSource _cts;

    /// <summary>
    /// Stores additional data of a tile at a specific coord.
    /// </summary>
    private readonly Dictionary<LandscapeCoord, LandscapeHeightmapData> _heightmapDataByCoord;

    /// <summary>
    /// Epsilon for detecting height differences.
    /// Used to make packets smaller by preventing sending very small height differences over the network.
    /// </summary>
    private const float Epsilon = 0.00001f;

    public SharedNetHeightmapManager(ILogger<SharedNetHeightmapManager> logger)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();
        _heightmapDataByCoord = new Dictionary<LandscapeCoord, LandscapeHeightmapData>();

        AsyncLoop(_cts.Token).Forget();
        SyncHeightmapLoop(_cts.Token).Forget();
    }

    public void OnWriteHeightmap(Bounds worldBounds, ref Landscape.LandscapeWriteHeightmapHandler callback)
    {
        var originalCallback = callback;
        LandscapeHeightmapData? cachedData = null;

        callback = (tileCoord, heightmapCoord, worldPosition, currentHeight) =>
        {
            cachedData = GetLandscapeHeightmapData(tileCoord, cachedData);

            var pendingDifference = cachedData.DeferredApplyDifference.Value[heightmapCoord.x, heightmapCoord.y];
            var currentPendingHeight = currentHeight + pendingDifference;

            var newHeight = Mathf.Clamp01(
                originalCallback(tileCoord, heightmapCoord, worldPosition, currentPendingHeight)
            );

            var difference = newHeight - currentPendingHeight;

            if (Math.Abs(difference) < Epsilon)
            {
                return currentHeight;
            }

            ApplyLocalEditDifference(cachedData, heightmapCoord, difference);

            // New height will be set later, but from this callback we return the old one so it won't be set again.
            return currentHeight;
        };
    }

    /// <summary>
    /// Registers height <paramref name="difference"/> at the given <paramref name="heightmapCoord"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="heightmapCoord"></param>
    /// <param name="difference"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // for performance, called from hot path
    private void ApplyLocalEditDifference(LandscapeHeightmapData data, HeightmapCoord heightmapCoord, float difference)
    {
        data.DeferredApplyDifference.Value[heightmapCoord.x, heightmapCoord.y] += difference;
        data.DeferredApplyDifference.MarkDirty();

        if (data.CurrentTransactionDifference != null)
        {
            data.CurrentTransactionDifference[heightmapCoord.x, heightmapCoord.y] += difference;
        }
    }

    public void ApplyDifference(LandscapeTile tile, float[,] differences)
    {
        var data = GetLandscapeHeightmapData(tile.coord);
        data.DeferredApplyDifference.MarkDirty();

        FastArrayOperations.AssignSum(
            first: data.DeferredApplyDifference,
            second: differences,
            destination: data.DeferredApplyDifference
        );
    }

    public void UnapplyDifference(LandscapeTile tile, float[,] differences)
    {
        var data = GetLandscapeHeightmapData(tile.coord);
        data.DeferredApplyDifference.MarkDirty();

        FastArrayOperations.AssignSubtract(
            first: data.DeferredApplyDifference,
            second: differences,
            destination: data.DeferredApplyDifference
        );
    }

    public void ApplyDifferenceLocal(LandscapeTile tile, float[,] differences)
    {
        FastArrayOperations.AssignSum(first: tile.heightmap, second: differences, destination: tile.heightmap);

        var data = GetLandscapeHeightmapData(tile.coord);
        data.NeedsHeightmapSync = true;

        var landscapeBounds = new LandscapeBounds(
            newMin: new LandscapeCoord(tile.coord.x - 1, tile.coord.y - 1),
            newMax: new LandscapeCoord(tile.coord.x + 1, tile.coord.y + 1)
        );

        LandscapeTileExtensions.UpdateTilesBorderHeights(landscapeBounds);

        // Already called by LandscapeTileExtensions.UpdateTilesBorderHeights(...)
        //tile.SetHeightsDelayLOD();

        // Called periodically but not here for performance
        //tile.SyncHeightmap();
    }

    public void BeginTransaction(LandscapeCoord landscapeCoord)
    {
        var data = GetLandscapeHeightmapData(landscapeCoord);

        if (data.CurrentTransactionDifference != null)
        {
            throw new InvalidOperationException("Transaction is already in progress");
        }

        data.CurrentTransactionDifference = LandscapeHeightmapCopyPoolEx.ClaimClean();
    }

    public float[,] EndTransaction(LandscapeCoord landscapeCoord)
    {
        var data = GetLandscapeHeightmapData(landscapeCoord);

        if (data.CurrentTransactionDifference == null)
        {
            throw new InvalidOperationException("Transaction was not began");
        }

        var differences = data.CurrentTransactionDifference;
        data.CurrentTransactionDifference = null;
        return differences;
    }

    public void EndAllTransactions()
    {
        var heightmapsToClear = _heightmapDataByCoord
            .Where(kvp => kvp.Value.CurrentTransactionDifference != null)
            .Select(kvp => kvp.Value);

        foreach (var data in heightmapsToClear)
        {
            data.CurrentTransactionDifference = null;
        }
    }

    /// <summary>
    /// Gets existing or creates a new instance of the <see cref="LandscapeHeightmapData"/>
    /// at the specified <paramref name="tileCoord"/>.
    /// </summary>
    /// <param name="tileCoord"></param>
    /// <param name="cachedData"></param>
    /// <returns></returns>
    private LandscapeHeightmapData GetLandscapeHeightmapData(
        LandscapeCoord tileCoord,
        LandscapeHeightmapData? cachedData = null)
    {
        // Cache the result for performance.
        if (cachedData != null && cachedData.TileCoord == tileCoord)
        {
            return cachedData;
        }

        if (_heightmapDataByCoord.TryGetValue(tileCoord, out var data))
        {
            return data;
        }

        var heightmap = LandscapeHeightmapCopyPoolEx.ClaimClean();

        data = new LandscapeHeightmapData(tileCoord)
        {
            DeferredApplyDifference = heightmap, //
            CurrentTransactionDifference = null
        };

        _heightmapDataByCoord.Add(tileCoord, data);
        return data;
    }

    /// <summary>
    /// Worker task that periodically synchronizes pending changes.
    /// </summary>
    /// <param name="ct"></param>
    private async UniTaskVoid AsyncLoop(CancellationToken ct)
    {
        // 10 times per second
        var interval = TimeSpan.FromMilliseconds(100);

        await foreach (var _ in UniTaskAsyncEnumerable.Interval(interval).WithCancellation(ct))
        {
            try
            {
                SendDifferences(_heightmapDataByCoord);
                ClearDifferences();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occured in {nameof(AsyncLoop)}");
            }
        }
    }

    /// <summary>
    /// Sends the difference to the server or to clients.
    /// </summary>
    /// <param name="heightmapDataByCoord"></param>
    protected abstract void SendDifferences(
        IReadOnlyDictionary<LandscapeCoord, LandscapeHeightmapData> heightmapDataByCoord
    );

    /// <summary>
    /// Cleans all dirty heightmaps.
    /// </summary>
    private void ClearDifferences()
    {
        var heightmapsToClear = _heightmapDataByCoord
            .Where(kvp => kvp.Value.DeferredApplyDifference.IsDirty)
            .Select(kvp => kvp.Value);

        foreach (var data in heightmapsToClear)
        {
            Array.Clear(data.DeferredApplyDifference, 0, data.DeferredApplyDifference.Value.Length);
            data.DeferredApplyDifference.MarkNonDirty();
        }
    }

    private async UniTaskVoid SyncHeightmapLoop(CancellationToken ct)
    {
        // 2 times per second
        var interval = TimeSpan.FromMilliseconds(500);

        await foreach (var _ in UniTaskAsyncEnumerable.Interval(interval).WithCancellation(ct))
        {
            try
            {
                SyncHeightmaps();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occured in {nameof(SyncHeightmapLoop)}");
            }
        }
    }

    private void SyncHeightmaps()
    {
        var heightmapsToSync = _heightmapDataByCoord
            .Where(kvp => kvp.Value.NeedsHeightmapSync)
            .Select(kvp => kvp.Value);

        foreach (var data in heightmapsToSync)
        {
            var tile = Landscape.getTile(data.TileCoord);
            tile.SyncHeightmap();
            data.NeedsHeightmapSync = false;
        }
    }

    ~SharedNetHeightmapManager()
    {
        _cts.Dispose();

        ReleaseHeightmaps();
    }

    public virtual void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();

        ReleaseHeightmaps();
    }

    /// <summary>
    /// Releases all claimed heightmaps back into the pool.
    /// </summary>
    private void ReleaseHeightmaps()
    {
        foreach (var kvp in _heightmapDataByCoord)
        {
            LandscapeHeightmapCopyPool.release(kvp.Value.DeferredApplyDifference);
        }

        _heightmapDataByCoord.Clear();
    }
}
