using LiteNetLib.Utils;

namespace MultiplayerMapEditor.Networking.UnityEngine;

public static class NetDataWriterExtensions
{
    public static void Put(this NetDataWriter writer, global::UnityEngine.Vector3 vector3)
    {
        writer.Put(vector3.x);
        writer.Put(vector3.y);
        writer.Put(vector3.z);
    }

    public static void Put(this NetDataWriter writer, global::UnityEngine.Quaternion quaternion)
    {
        writer.Put(quaternion.x);
        writer.Put(quaternion.y);
        writer.Put(quaternion.z);
        writer.Put(quaternion.w);
    }
}
