namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// Determines the state of a <see cref="IReun"/>.
/// </summary>
internal enum ReunState
{
    /// <summary>
    /// Redo() was called.
    /// </summary>
    Redo,

    /// <summary>
    /// Waiting for a response (e.g. from a server).
    /// </summary>
    Waiting,

    /// <summary>
    /// Undo() was called.
    /// </summary>
    Undo,
}
