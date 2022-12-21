namespace MultiplayerMapEditor.Editor.Level.Objects;

internal static class LevelObjectsReunLinker
{
    /// <summary>
    /// Replaces <see cref="NetId"/> on all registered <see cref="INetReun"/>s.
    ///
    /// Lets look at the example:
    ///     1. Create a new object.
    ///     2. Undo.
    ///     3. Redo.
    /// On step 3 a new NetId is assigned, so we need to notify other <see cref="INetReun"/>s about that.
    /// </summary>
    /// <param name="oldNetId"><see cref="NetId"/> to replace.</param>
    /// <param name="newNetId"><see cref="NetId"/> to replace with.</param>
    public static void ReplaceNetId(NetId oldNetId, NetId newNetId)
    {
        var reuns = LevelObjectsReflection.GetReuns()
            .OfType<INetReun>()
            .Where(reun => reun.NetId == oldNetId);

        foreach (var reun in reuns)
        {
            reun.NetId = newNetId;
        }
    }
}
