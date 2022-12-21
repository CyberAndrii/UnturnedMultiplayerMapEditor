using System.Diagnostics.CodeAnalysis;

namespace MultiplayerMapEditor.Editor.Level.Objects;

internal interface ILevelObjectsManager
{
    LevelObjectWrapper? Create(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        ushort id,
        Guid guid,
        ELevelObjectPlacementOrigin placementOrigin,
        uint? instanceId,
        NetId netId,
        AssetReference<MaterialPaletteAsset> materialPalette,
        int? materialIndexOverride,
        bool select
    );

    LevelBuildableObjectWrapper? CreateBuildable(
        Vector3 position,
        Quaternion rotation,
        ushort id,
        NetId netId,
        bool select
    );

    void TransformObject(NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale
    );

    void Remove(NetId netId);

    bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out NetId? netId);

    bool TryFind(NetId netId, [NotNullWhen(true)] out LevelObject? levelObject);

    bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out LevelObject? levelObject);

    bool TryFind(NetId netId, [NotNullWhen(true)] out LevelBuildableObject? levelBuildableObject);

    bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out LevelBuildableObject? buildableObject);
}
