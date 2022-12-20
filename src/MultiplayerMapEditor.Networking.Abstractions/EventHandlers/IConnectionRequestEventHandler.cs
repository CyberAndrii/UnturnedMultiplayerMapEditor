namespace MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

public interface IConnectionRequestEventHandler
{
    void OnConnectionRequest(ConnectionRequest request);
}
