using MultiplayerMapEditor.Abstractions.Level;

namespace MultiplayerMapEditor.Level;

internal sealed class UnturnedLevelManager : ILevelManager, IDisposable
{
    public event System.Action? OnLevelExit;

    public UnturnedLevelManager()
    {
        SDG.Unturned.Level.onLevelExited += OnLevelExited;
    }

    public ILevelInfo? CurrentLevelInfo => SDG.Unturned.Level.info == null
        ? null
        : new UnturnedLevelInfo(SDG.Unturned.Level.info);

    public ILevelInfo? Find(string name)
    {
        var unturnedLevel = SDG.Unturned.Level.getLevel(name);
        return unturnedLevel == null ? null : new UnturnedLevelInfo(unturnedLevel);
    }

    public void LoadLevel(ILevelInfo levelInfo)
    {
        var unturnedLevelInfo = ((UnturnedLevelInfo)levelInfo).LevelInfo;

        MenuSettings.save(); // called in the original code before loading the editor
        SDG.Unturned.Level.edit(unturnedLevelInfo);
    }

    public void Exit()
    {
        SDG.Unturned.Level.exit();
    }

    public void Dispose()
    {
        SDG.Unturned.Level.onLevelExited -= OnLevelExited;
    }

    private void OnLevelExited()
    {
        OnLevelExit?.Invoke();
    }
}
