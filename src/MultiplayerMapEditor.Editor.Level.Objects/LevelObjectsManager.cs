using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;

namespace MultiplayerMapEditor.Editor.Level.Objects;

internal sealed class LevelObjectsManager : ILevelObjectsManager
{
    private readonly ILogger<LevelObjectsManager> _logger;

    public LevelObjectsManager(ILogger<LevelObjectsManager> logger)
    {
        _logger = logger;
    }

    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/cc326e1e66c2d2bdc228a196505c41047056c2fe/Assembly-CSharp/SDG.Unturned/LevelObjects.cs#L302
    public LevelObjectWrapper? Create(
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
        bool select)
    {
        if (!Regions.tryGetCoordinate(position, out var x, out var y))
        {
            return null;
        }

        var @object = LevelObjectReflection.New(
            position,
            rotation,
            scale,
            id,
            guid,
            placementOrigin,
            instanceId ?? LevelObjectsReflection.GenerateUniqueInstanceId(),
            materialPalette,
            materialIndexOverride ?? -1,
            netId
        );

        @object.enableCollision();
        @object.enableVisual();
        @object.disableSkybox();

        LevelObjects.objects[x, y].Add(@object);
        LevelObjectsReflection.IncrementTotalCount();

        if (select)
        {
            EditorObjects.addSelection(@object.transform);
        }

        @object.ReapplyMaterialOverrides();

        _logger.LogObjectCreated(@object.asset.name, netId);

        return new LevelObjectWrapper(@object, netId);
    }

    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/cc326e1e66c2d2bdc228a196505c41047056c2fe/Assembly-CSharp/SDG.Unturned/LevelObjects.cs#L317
    public LevelBuildableObjectWrapper? CreateBuildable(
        Vector3 position,
        Quaternion rotation,
        ushort id,
        NetId netId,
        bool select)
    {
        if (!Regions.tryGetCoordinate(position, out var x, out var y))
        {
            return null;
        }

        var buildableObject = new LevelBuildableObject(position, rotation, id);
        buildableObject.enable();

        LevelObjects.buildables[x, y].Add(buildableObject);
        LevelObjectsReflection.IncrementTotalCount();

        // Unlike LevelObject, LevelBuildableObject is not automatically registered by the game
        NetIdRegistry.AssignTransform(netId, buildableObject.transform);

        if (select)
        {
            EditorObjects.addSelection(buildableObject.transform);
        }

        _logger.LogBuildableObjectCreated(buildableObject.asset.name, netId);
        return new LevelBuildableObjectWrapper(buildableObject, netId);
    }

    public void TransformObject(NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale)
    {
        var transform = NetIdRegistry.GetTransform(netId, null);

        if (transform == null)
        {
            throw new ArgumentException("Transform was not found", nameof(netId))
            {
                Data = { ["NetId"] = netId.ToString() }
            };
        }

        LevelObjects.transformObject(
            transform,
            toPosition,
            toRotation,
            toScale,
            fromPosition,
            fromRotation,
            fromScale
        );

        // This is needed to sync pivot on other players when multiple players selected the same object.
        EditorObjectsReflection.CalculateHandleOffsets();
    }

    public void Remove(NetId netId)
    {
        if (TryFind(netId, out LevelObject? @object))
        {
            Remove(@object, netId);
            return;
        }

        if (TryFind(netId, out LevelBuildableObject? buildableObject))
        {
            RemoveBuildable(@buildableObject, netId);
            return;
        }

        throw new ArgumentException("Level object was not found", nameof(netId))
        {
            Data = { ["NetId"] = netId.ToString() }
        };
    }


    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/cc326e1e66c2d2bdc228a196505c41047056c2fe/Assembly-CSharp/SDG.Unturned/LevelObjects.cs#L335
    private void Remove(LevelObject @object, NetId netId)
    {
        if (@object.transform == null ||
            !Regions.tryGetCoordinate(@object.transform.position, out var x, out var y))
        {
            return;
        }

        @object.destroy();

        var regionObjects = LevelObjects.objects[x, y];
        regionObjects.Remove(@object);

        LevelObjectsReflection.DecrementTotalCount();
        NetIdRegistry.ReleaseTransform(netId, @object.transform);

        Deselect(@object.transform);

        _logger.LogObjectRemoved(@object.asset.name, netId);
    }

    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/cc326e1e66c2d2bdc228a196505c41047056c2fe/Assembly-CSharp/SDG.Unturned/LevelObjects.cs#L353
    private void RemoveBuildable(LevelBuildableObject buildableObject, NetId netId)
    {
        if (buildableObject.transform == null ||
            !Regions.tryGetCoordinate(buildableObject.transform.position, out var x, out var y))
        {
            return;
        }

        buildableObject.destroy();

        var regionObjects = LevelObjects.buildables[x, y];
        regionObjects.Remove(buildableObject);

        LevelObjectsReflection.DecrementTotalCount();
        NetIdRegistry.ReleaseTransform(netId, buildableObject.transform);

        Deselect(buildableObject.transform);

        _logger.LogBuildableObjectRemoved(buildableObject.asset.name, netId);
    }

    private void Deselect(UnityEngine.Transform transform)
    {
        // Delay to prevent changing collection if iteration of selections is not yet finished (by Unturned code).
        // Fixes a bug when some of selected objects will not get deleted.
        // Using custom method because the original throws NullReferenceException.
        UniTask.Post(() => EditorObjectsReflection.RemoveSelection(transform), PlayerLoopTiming.LastUpdate);
    }

    public bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out NetId? netId)
    {
        if (transform == null)
        {
            throw new ArgumentNullException(nameof(transform));
        }

        if (!NetIdRegistry.GetTransformNetId(transform, out var netIdOut, out var path))
        {
            netId = null;
            return false;
        }

        netId = netIdOut;
        return netId != NetId.INVALID;
    }

    public bool TryFind(NetId netId, [NotNullWhen(true)] out LevelObject? @object)
    {
        if (netId == NetId.INVALID)
        {
            throw new ArgumentException("NetId is invalid", nameof(netId));
        }

        @object = null;
        var transform = NetIdRegistry.Get<UnityEngine.Transform?>(netId);
        return transform != null && TryFind(transform, out @object);
    }

    public bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out LevelObject? @object)
    {
        if (transform == null)
        {
            throw new ArgumentNullException(nameof(transform));
        }

        if (!Regions.tryGetCoordinate(transform.position, out var x, out var y))
        {
            @object = null;
            return false;
        }

        var regionObjects = LevelObjects.objects[x, y];
        @object = regionObjects.FirstOrDefault(obj => obj.transform == transform);
        return @object != null;
    }

    public bool TryFind(NetId netId, [NotNullWhen(true)] out LevelBuildableObject? buildableObject)
    {
        if (netId == NetId.INVALID)
        {
            throw new ArgumentException("NetId is invalid", nameof(netId));
        }

        buildableObject = null;
        var transform = NetIdRegistry.Get<UnityEngine.Transform?>(netId);
        return transform != null && TryFind(transform, out buildableObject);
    }

    public bool TryFind(UnityEngine.Transform transform, [NotNullWhen(true)] out LevelBuildableObject? buildableObject)
    {
        if (transform == null)
        {
            throw new ArgumentNullException(nameof(transform));
        }

        if (!Regions.tryGetCoordinate(transform.position, out var x, out var y))
        {
            buildableObject = null;
            return false;
        }

        var regionObjects = LevelObjects.buildables[x, y];
        buildableObject = regionObjects.FirstOrDefault(obj => obj.transform == transform);
        return buildableObject != null;
    }
}
