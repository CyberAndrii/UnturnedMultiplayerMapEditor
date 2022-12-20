using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// Allows to access private members of the <see cref="LevelObjects"/> class.
/// </summary>
/// <remarks>Keep in mind that these operations are not atomic.</remarks>
internal static class LevelObjectsReflection
{
    private static readonly FieldInfo TotalObjectsField =
        typeof(LevelObjects).GetField("_total", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObjects), "_total");

    private static readonly FieldInfo ReunField =
        typeof(LevelObjects).GetField("reun", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObjects), "reun");

    private static readonly FieldInfo FrameField =
        typeof(LevelObjects).GetField("frame", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObjects), "frame");

    private static readonly MethodInfo GenerateUniqueInstanceIdMethod =
        typeof(LevelObjects).GetMethod("generateUniqueInstanceID", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObjects), "generateUniqueInstanceID");

    public static int GetTotalObjects()
    {
        return (int)TotalObjectsField.GetValue(null);
    }

    public static void SetTotalObjects(int value)
    {
        TotalObjectsField.SetValue(null, value);
    }

    public static void IncrementTotalCount(int amount = 1)
    {
        var newTotal = GetTotalObjects() + amount;
        SetTotalObjects(newTotal);
    }

    public static void DecrementTotalCount(int amount = 1)
    {
        var newTotal = GetTotalObjects() - amount;
        SetTotalObjects(newTotal);
    }

    public static IReun[] GetReuns()
    {
        return (IReun[])ReunField.GetValue(null);
    }

    public static int GetFrame()
    {
        return (int)FrameField.GetValue(null);
    }

    /// <remarks>
    /// WARNING: After every level load it starts from zero so it's not really unique.
    /// Multiple objects can have the same id.
    /// </remarks>
    public static uint GenerateUniqueInstanceId()
    {
        return (uint)GenerateUniqueInstanceIdMethod.Invoke(null, Array.Empty<object>());
    }
}
