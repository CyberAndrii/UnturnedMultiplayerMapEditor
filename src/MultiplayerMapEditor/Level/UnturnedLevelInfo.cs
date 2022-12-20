using MultiplayerMapEditor.Abstractions.Level;

namespace MultiplayerMapEditor.Level;

internal sealed class UnturnedLevelInfo : ILevelInfo
{
    public UnturnedLevelInfo(SDG.Unturned.LevelInfo levelInfo)
    {
        LevelInfo = levelInfo;
    }

    public SDG.Unturned.LevelInfo LevelInfo { get; }

    public string Name => LevelInfo.name;

    public string Path => LevelInfo.path;
}
