namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface INetworkReceiveEventHandler
{
    void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
}
