using MultiplayerMapEditor.Networking.Abstractions.Packets;

namespace MultiplayerMapEditor.Networking.UnityEngine;

public static class NetPacketProcessorExtensions
{
    public static void RegisterVector3(this INetPacketProcessor netPacketProcessor)
    {
        netPacketProcessor.RegisterNestedType(
            (writer, vector3) => writer.Put(vector3),
            reader => reader.GetVector3()
        );
    }

    public static void RegisterQuaternion(this INetPacketProcessor netPacketProcessor)
    {
        netPacketProcessor.RegisterNestedType(
            (writer, quaternion) => writer.Put(quaternion),
            reader => reader.GetQuaternion()
        );
    }
}
