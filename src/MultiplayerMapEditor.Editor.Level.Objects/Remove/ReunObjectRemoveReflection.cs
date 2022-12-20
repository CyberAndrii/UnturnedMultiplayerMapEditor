using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

internal static class ReunObjectRemoveReflection
{
    private static readonly FieldInfo PositionField =
        typeof(ReunObjectRemove).GetField("position", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "position");

    private static readonly FieldInfo RotationField =
        typeof(ReunObjectRemove).GetField("rotation", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "rotation");

    private static readonly FieldInfo ScaleField =
        typeof(ReunObjectRemove).GetField("scale", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "scale");

    private static readonly FieldInfo ObjectAssetField =
        typeof(ReunObjectRemove).GetField("objectAsset", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "objectAsset");

    private static readonly FieldInfo ItemAssetField =
        typeof(ReunObjectRemove).GetField("itemAsset", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "itemAsset");

    private static readonly FieldInfo ModelField =
        typeof(ReunObjectRemove).GetField("model", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectRemove), "model");

    public static Vector3 GetPosition(this ReunObjectRemove reun)
    {
        return (Vector3)PositionField.GetValue(reun);
    }

    public static Quaternion GetRotation(this ReunObjectRemove reun)
    {
        return (Quaternion)RotationField.GetValue(reun);
    }

    public static Vector3 GetScale(this ReunObjectRemove reun)
    {
        return (Vector3)ScaleField.GetValue(reun);
    }

    public static ObjectAsset? GetObjectAsset(this ReunObjectRemove reun)
    {
        return (ObjectAsset?)ObjectAssetField.GetValue(reun);
    }

    public static ItemAsset? GetItemAsset(this ReunObjectRemove reun)
    {
        return (ItemAsset?)ItemAssetField.GetValue(reun);
    }

    public static Transform? GetModel(this ReunObjectRemove reun)
    {
        return (Transform?)ModelField.GetValue(reun);
    }
}
