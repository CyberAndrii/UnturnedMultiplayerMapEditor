using System.Diagnostics.Contracts;

namespace MultiplayerMapEditor.Common;

/// <summary>
/// Represents a value type that stores dirt state of an object.
/// </summary>
/// <typeparam name="T">Type of dirtyable object.</typeparam>
internal readonly struct Dirtyable<T>
{
    public Dirtyable(T value, bool isDirty)
    {
        Value = value;
        IsDirty = isDirty;
    }

    /// <summary>
    /// Object that can be marked as dirty.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Determines whether <see cref="Value"/> is dirty.
    /// </summary>
    public bool IsDirty { get; private init; }

    /// <summary>
    /// Returns a new instance of <see cref="Dirtyable{T}"/> with <see cref="IsDirty"/> set to <see langword="true"/>.
    /// </summary>
    /// <returns></returns>
    [Pure]
    public Dirtyable<T> AsDirty()
    {
        return this with { IsDirty = true };
    }

    /// <summary>
    /// Returns a new instance of <see cref="Dirtyable{T}"/> with <see cref="IsDirty"/> set to <see langword="false"/>.
    /// </summary>
    /// <returns></returns>
    [Pure]
    public Dirtyable<T> AsNonDirty()
    {
        return this with { IsDirty = false };
    }

    public static implicit operator T(Dirtyable<T> value)
    {
        return value.Value;
    }

    public static implicit operator Dirtyable<T>(T value)
    {
        return new Dirtyable<T>(value, false);
    }
}
