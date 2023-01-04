using System.Diagnostics.Contracts;

namespace MultiplayerMapEditor.Common;

internal static class DirtyableExtensions
{
    [Pure]
    public static Dirtyable<T> AsDirtyable<T>(this T value, bool isDirty = false)
    {
        return new Dirtyable<T>(value, isDirty);
    }

    public static void MarkDirty<T>(ref this Dirtyable<T> dirtyable)
    {
        dirtyable = dirtyable.AsDirty();
    }

    public static void MarkNonDirty<T>(ref this Dirtyable<T> dirtyable)
    {
        dirtyable = dirtyable.AsNonDirty();
    }
}
