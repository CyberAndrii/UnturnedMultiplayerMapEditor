namespace MultiplayerMapEditor.Abstractions.Level;

public interface ILevelManager
{
    event System.Action? OnLevelExit;

    ILevelInfo? CurrentLevelInfo { get; }

    ILevelInfo? Find(string name);

    void LoadLevel(ILevelInfo levelInfo);

    void Exit();
}
