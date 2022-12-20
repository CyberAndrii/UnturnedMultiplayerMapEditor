namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class NetworkReceiveEventHandler : INetworkReceiveEventHandler
{
    private readonly INetPacketProcessor _netPacketProcessor;

    public NetworkReceiveEventHandler(INetPacketProcessor netPacketProcessor)
    {
        _netPacketProcessor = netPacketProcessor;
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        _netPacketProcessor.ReadAllPackets(reader, peer);
    }
}
