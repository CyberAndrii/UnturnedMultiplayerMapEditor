namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal interface INetObjectPropertiesUpdater
{
    void UpdateObjectProperties(NetId netId, Guid materialOverride, int materialIndexOverride);
}
