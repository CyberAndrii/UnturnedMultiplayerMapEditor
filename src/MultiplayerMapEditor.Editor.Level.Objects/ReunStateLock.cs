namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// A lock that prevents doing the same action (redo/undo) multiple times in a row.
/// Supports delayed callback.
/// </summary>
internal class ReunStateLock
{
    private ReunState _currentState;
    private readonly Func<ReunState, ReunState, bool> _canEnterFunc;

    /// <summary>
    /// Constructs a new instance of the <see cref="ReunStateLock"/> class.
    /// </summary>
    /// <param name="currentState">The state of the <see cref="IReun"/> at the time of instantiation of this class.</param>
    /// <param name="canEnterFunc">A callback function that determines whether a state change is allowed.</param>
    public ReunStateLock(ReunState currentState, Func<ReunState, ReunState, bool> canEnterFunc)
    {
        _currentState = currentState;
        _canEnterFunc = canEnterFunc;
    }

    /// <summary>
    /// Try to change the state to a <paramref name="newState"/>.
    /// </summary>
    /// <param name="newState">A state to change to.</param>
    /// <returns>True if changed, otherwise false.</returns>
    public bool TryEnter(ReunState newState)
    {
        if (_canEnterFunc(_currentState, newState))
        {
            _currentState = newState;
            return true;
        }

        return false;
    }
}
