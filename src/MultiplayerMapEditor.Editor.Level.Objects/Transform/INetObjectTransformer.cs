namespace MultiplayerMapEditor.Editor.Level.Objects.Transform;

internal interface INetObjectTransformer
{
    void TransformObject(
        NetId netId,
        Vector3 fromPosition,
        Quaternion fromRotation,
        Vector3 fromScale,
        Vector3 toPosition,
        Quaternion toRotation,
        Vector3 toScale
    );
}
