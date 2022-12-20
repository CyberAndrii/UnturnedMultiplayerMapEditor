using LiteNetLib.Utils;
using SDG.Unturned;

namespace MultiplayerMapEditor.Networking.Unturned;

public static class NetDataReaderExtensions
{
    public static NetId GetNetId(this NetDataReader reader)
    {
        return new NetId(reader.GetUInt());
    }
}
