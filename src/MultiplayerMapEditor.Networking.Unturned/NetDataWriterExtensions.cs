using LiteNetLib.Utils;
using SDG.Unturned;

namespace MultiplayerMapEditor.Networking.Unturned;

public static class NetDataWriterExtensions
{
    public static void Put(this NetDataWriter writer, NetId net)
    {
        writer.Put(net.id);
    }
}
