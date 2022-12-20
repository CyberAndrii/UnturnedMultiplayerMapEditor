namespace MultiplayerMapEditor.Editor.Level.Objects;

internal record LevelBuildableObjectWrapper(
    LevelBuildableObject? BuildableObject,
    NetId NetId
);
