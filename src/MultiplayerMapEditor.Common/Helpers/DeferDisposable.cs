namespace MultiplayerMapEditor.Common.Helpers;

internal readonly struct DeferDisposable : IDisposable
{
    private readonly Action _action;

    private DeferDisposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }

    public static DeferDisposable Defer(Action action) => new(action);
}
