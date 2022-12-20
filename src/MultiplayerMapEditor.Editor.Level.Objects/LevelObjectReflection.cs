using System.Reflection;
using System.Runtime.Serialization;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects;

/// <summary>
/// Allows to access private members of the <see cref="LevelObject"/> class.
/// </summary>
internal static class LevelObjectReflection
{
    private static readonly FieldInfo CustomMaterialOverrideField =
        typeof(LevelObject).GetField("customMaterialOverride", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObject), "customMaterialOverride");

    private static readonly FieldInfo MaterialIndexOverrideField =
        typeof(LevelObject).GetField("materialIndexOverride", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObject), "materialIndexOverride");

    private static readonly MethodInfo ReapplyMaterialOverridesMethod =
        typeof(LevelObject).GetMethod("ReapplyMaterialOverrides", Instance | NonPublic)
        ?? throw new MissingMemberException(nameof(LevelObject), "ReapplyMaterialOverrides");

    private static readonly ConstructorInfo Constructor =
        typeof(LevelObject).GetConstructor(
            Instance | NonPublic,
            null,
            new[]
            {
                typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(ushort), typeof(Guid),
                typeof(ELevelObjectPlacementOrigin), typeof(uint), typeof(AssetReference<MaterialPaletteAsset>),
                typeof(int), typeof(NetId)
            },
            null)
        ?? throw new MissingMemberException(nameof(LevelObject), "Constructor");

    public static Guid GetCustomMaterialOverrideGuid(this LevelObject levelObject)
    {
        var assetRef = (AssetReference<MaterialPaletteAsset>)CustomMaterialOverrideField.GetValue(levelObject);
        return assetRef.GUID;
    }

    public static int GetMaterialIndexOverride(this LevelObject levelObject)
    {
        return (int)MaterialIndexOverrideField.GetValue(levelObject);
    }

    public static void ReapplyMaterialOverrides(this LevelObject levelObject)
    {
        ReapplyMaterialOverridesMethod.Invoke(levelObject, Array.Empty<object>());
    }

    public static LevelObject New(
        Vector3 point,
        Quaternion rotation,
        Vector3 scale,
        ushort id,
        Guid guid,
        ELevelObjectPlacementOrigin placementOrigin,
        uint instanceId,
        AssetReference<MaterialPaletteAsset> customMaterialOverride,
        int materialIndexOverride,
        NetId netId)
    {
        var obj = (LevelObject)FormatterServices.GetUninitializedObject(typeof(LevelObject));

        var args = new object[]
        {
            point, rotation, scale, id, guid, placementOrigin, instanceId, customMaterialOverride,
            materialIndexOverride, netId
        };

        Constructor.Invoke(obj, args);

        return obj;
    }
}
