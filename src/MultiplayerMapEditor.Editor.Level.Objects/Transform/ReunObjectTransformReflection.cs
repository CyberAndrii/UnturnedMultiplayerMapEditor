using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

/// <summary>
/// Allows to access private members of the <see cref="SDG.Unturned.ReunObjectTransform"/> class.
/// </summary>
internal static class ReunObjectTransformReflection
{
    private static readonly FieldInfo FromPositionField =
        typeof(ReunObjectTransform).GetField("fromPosition", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "fromPosition");

    private static readonly FieldInfo FromRotationField =
        typeof(ReunObjectTransform).GetField("fromRotation", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "fromRotation");

    private static readonly FieldInfo FromScaleField =
        typeof(ReunObjectTransform).GetField("fromScale", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "fromScale");

    private static readonly FieldInfo ToPositionField =
        typeof(ReunObjectTransform).GetField("toPosition", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "toPosition");

    private static readonly FieldInfo ToRotationField =
        typeof(ReunObjectTransform).GetField("toRotation", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "toRotation");

    private static readonly FieldInfo ToScaleField =
        typeof(ReunObjectTransform).GetField("toScale", Instance | NonPublic)
        ?? throw new MissingFieldException(nameof(ReunObjectTransform), "toScale");

    private static readonly FieldInfo ModelField =
        typeof(ReunObjectTransform).GetField("model", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectTransform), "model");

    public static Vector3 GetFromPosition(this ReunObjectTransform reun)
    {
        return (Vector3)FromPositionField.GetValue(reun);
    }

    public static Quaternion GetFromRotation(this ReunObjectTransform reun)
    {
        return (Quaternion)FromRotationField.GetValue(reun);
    }

    public static Vector3 GetFromScale(this ReunObjectTransform reun)
    {
        return (Vector3)FromScaleField.GetValue(reun);
    }

    public static Vector3 GetToPosition(this ReunObjectTransform reun)
    {
        return (Vector3)ToPositionField.GetValue(reun);
    }

    public static Quaternion GetToRotation(this ReunObjectTransform reun)
    {
        return (Quaternion)ToRotationField.GetValue(reun);
    }

    public static Vector3 GetToScale(this ReunObjectTransform reun)
    {
        return (Vector3)ToScaleField.GetValue(reun);
    }

    public static UnityEngine.Transform? GetModel(this ReunObjectTransform reun)
    {
        return (UnityEngine.Transform?)ModelField.GetValue(reun);
    }
}
