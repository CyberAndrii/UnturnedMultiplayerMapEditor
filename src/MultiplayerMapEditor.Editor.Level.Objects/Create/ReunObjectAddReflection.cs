using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects.Create;

internal static class ReunObjectAddReflection
{
    private static readonly FieldInfo PositionField =
        typeof(ReunObjectAdd).GetField("position", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectAdd), "position");

    private static readonly FieldInfo RotationField =
        typeof(ReunObjectAdd).GetField("rotation", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectAdd), "rotation");

    private static readonly FieldInfo ScaleField =
        typeof(ReunObjectAdd).GetField("scale", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectAdd), "scale");

    private static readonly FieldInfo ObjectAssetField =
        typeof(ReunObjectAdd).GetField("objectAsset", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectAdd), "objectAsset");

    private static readonly FieldInfo ItemAssetField =
        typeof(ReunObjectAdd).GetField("itemAsset", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(ReunObjectAdd), "itemAsset");

    public static Vector3 GetPosition(this ReunObjectAdd reun)
    {
        return (Vector3)PositionField.GetValue(reun);
    }

    public static Quaternion GetRotation(this ReunObjectAdd reun)
    {
        return (Quaternion)RotationField.GetValue(reun);
    }

    public static Vector3 GetScale(this ReunObjectAdd reun)
    {
        return (Vector3)ScaleField.GetValue(reun);
    }

    public static ObjectAsset? GetObjectAsset(this ReunObjectAdd reun)
    {
        return (ObjectAsset?)ObjectAssetField.GetValue(reun);
    }

    public static ItemAsset? GetItemAsset(this ReunObjectAdd reun)
    {
        return (ItemAsset?)ItemAssetField.GetValue(reun);
    }
}
