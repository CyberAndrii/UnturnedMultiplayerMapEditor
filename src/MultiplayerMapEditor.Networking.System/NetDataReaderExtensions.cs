using LiteNetLib.Utils;

namespace MultiplayerMapEditor.Networking.System;

public static class NetDataReaderExtensions
{
    public static T? GetNullable<T>(this NetDataReader reader, Func<NetDataReader, T> readerFunc) where T : struct
    {
        return reader.GetBool() ? readerFunc(reader) : null;
    }

    public static T? GetOrDefault<T>(this NetDataReader reader, Func<NetDataReader, T> readerFunc)
    {
        return reader.GetBool() ? readerFunc(reader) : default;
    }

    public static Guid GetGuid(this NetDataReader reader)
    {
        return reader.GetGuidBuffer().Guid;
    }

    internal static GuidBuffer GetGuidBuffer(this NetDataReader reader)
    {
        return new GuidBuffer(reader.GetULong(), reader.GetULong());
    }
}
