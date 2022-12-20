using LiteNetLib.Utils;

namespace MultiplayerMapEditor.Networking.UnityEngine;

public static class NetDataReaderExtensions
{
    public static global::UnityEngine.Vector3 GetVector3(this NetDataReader reader)
    {
        return new global::UnityEngine.Vector3(
            x: reader.GetFloat(),
            y: reader.GetFloat(),
            z: reader.GetFloat()
        );
    }

    public static global::UnityEngine.Quaternion GetQuaternion(this NetDataReader reader)
    {
        return new global::UnityEngine.Quaternion(
            x: reader.GetFloat(),
            y: reader.GetFloat(),
            z: reader.GetFloat(),
            w: reader.GetFloat()
        );
    }
}
