using MultiplayerMapEditor.Networking.Abstractions.Packets;

namespace MultiplayerMapEditor.Networking.Unturned;

public static class NetPacketProcessorExtensions
{
    public static void RegisterNetId(this INetPacketProcessor netPacketProcessor)
    {
        netPacketProcessor.RegisterNestedType(
            (writer, netId) => writer.Put(netId),
            reader => reader.GetNetId()
        );
    }
}
