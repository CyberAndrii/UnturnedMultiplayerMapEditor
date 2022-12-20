namespace MultiplayerMapEditor.Networking.EventHandlers;

internal sealed class ClientsideConnectionRequestEventHandler : IConnectionRequestEventHandler
{
    public void OnConnectionRequest(ConnectionRequest request)
    {
        // Connecting to a client is disallowed.
        request.RejectForce();
    }
}

internal sealed class ServersideConnectionRequestEventHandler : IConnectionRequestEventHandler
{
    private readonly IClientRegistry? _clientRegistry;

    public ServersideConnectionRequestEventHandler(IClientRegistry clientRegistry)
    {
        _clientRegistry = clientRegistry;
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        _clientRegistry!.AcceptOrReject(request);
    }
}
