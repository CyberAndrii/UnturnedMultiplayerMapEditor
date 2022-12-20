namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// Contains <see cref="ILogger"/> extensions used in the <see cref="LevelObjectsManager"/> class.
/// </summary>
internal static partial class LevelObjectsManagerLoggerMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Trace,
        Message = "Created object {Name} with NetId {NetId}"
    )]
    internal static partial void LogObjectCreated(this ILogger logger, string name, NetId netId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Trace,
        Message = "Created buildable object {Name} with NetId {NetId}"
    )]
    internal static partial void LogBuildableObjectCreated(this ILogger logger, string name, NetId netId);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Trace,
        Message = "Removed object {Name} with NetId {NetId}"
    )]
    internal static partial void LogObjectRemoved(this ILogger logger, string name, NetId netId);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Trace,
        Message = "Removed buildable object {Name} with NetId {NetId}"
    )]
    internal static partial void LogBuildableObjectRemoved(this ILogger logger, string name, NetId netId);
}
