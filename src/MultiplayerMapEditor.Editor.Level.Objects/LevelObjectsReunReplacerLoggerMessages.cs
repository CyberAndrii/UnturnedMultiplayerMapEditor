namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// Contains <see cref="ILogger"/> extensions used in the <see cref="LevelObjectsReunReplacer"/> class.
/// </summary>
internal static partial class LevelObjectsReunReplacerLoggerMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Trace,
        Message = "Replaced reun {OriginalName} with {NewName}"
    )]
    internal static partial void LogReunReplaced(this ILogger logger, string originalName, string newName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Unknown reun {Name} was not replaced with networkable one"
    )]
    internal static partial void LogReunNotReplaced(this ILogger logger, string name);
}
