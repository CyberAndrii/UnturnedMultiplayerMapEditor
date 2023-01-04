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

    public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(
        this NetDataReader reader,
        Func<NetDataReader, TKey> keyReaderFunc,
        Func<NetDataReader, TValue> valueReaderFunc)
    {
        return reader.GetDictionary(lengthReader => lengthReader.GetUShort(), keyReaderFunc, valueReaderFunc);
    }

    public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(
        this NetDataReader reader,
        Func<NetDataReader, int> lengthReaderFunc,
        Func<NetDataReader, TKey> keyReaderFunc,
        Func<NetDataReader, TValue> valueReaderFunc)
    {
        var length = lengthReaderFunc(reader);
        var dictionary = new Dictionary<TKey, TValue>(length);

        for (var i = 0; i < length; i++)
        {
            var key = keyReaderFunc(reader);
            var value = valueReaderFunc(reader);
            dictionary[key] = value;
        }

        return dictionary;
    }
}
