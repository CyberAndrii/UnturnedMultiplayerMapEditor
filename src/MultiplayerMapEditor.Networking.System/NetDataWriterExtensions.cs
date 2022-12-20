using LiteNetLib.Utils;

namespace MultiplayerMapEditor.Networking.System;

public static class NetDataWriterExtensions
{
    public static void PutNullable<T>(this NetDataWriter writer, T? nullable, Action<NetDataWriter, T> writerAction)
        where T : struct
    {
        writer.Put(nullable.HasValue);

        if (!nullable.HasValue)
        {
            return;
        }

        writerAction(writer, nullable.Value);
    }

    public static void PutOrDefault<T>(this NetDataWriter writer, T? nullable, Action<NetDataWriter, T> writerAction)
    {
        writer.Put(nullable == null);

        if (nullable == null)
        {
            return;
        }

        writerAction(writer, nullable);
    }

    public static void Put(this NetDataWriter writer, Guid guid)
    {
        writer.Put(new GuidBuffer(guid));
    }

    internal static void Put(this NetDataWriter writer, GuidBuffer guidBuffer)
    {
        writer.Put(guidBuffer.Part1);
        writer.Put(guidBuffer.Part2);
    }
}
