using LiteNetLib.Utils;
using MultiplayerMapEditor.Networking.Abstractions.Packets;

namespace MultiplayerMapEditor.Networking.System;

public static class NetPacketProcessorExtensions
{
    public static void RegisterGuid(this INetPacketProcessor netPacketProcessor)
    {
        netPacketProcessor.RegisterNestedType(
            (writer, guid) => writer.Put(guid),
            reader => reader.GetGuid()
        );
    }

    public static void RegisterNullable<T>(
        this INetPacketProcessor netPacketProcessor,
        Action<NetDataWriter, T> writerAction,
        Func<NetDataReader, T> readerFunc)
        where T : struct
    {
        netPacketProcessor.RegisterNestedType(
            (writer, value) => writer.PutNullable(value, writerAction),
            reader => reader.GetNullable(readerFunc)
        );
    }
}
