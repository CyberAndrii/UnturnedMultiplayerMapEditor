namespace MultiplayerMapEditor.Networking.Abstractions;

/// <summary>
/// Manages client connections.
/// Only available on the server.
/// </summary>
public interface IClientRegistry
{
    /// <summary>
    /// Gets a list of connecting players.
    /// </summary>
    IReadOnlyList<ConnectingPlayer> ConnectingPlayers { get; }

    /// <summary>
    /// Decides whether to accept or reject the connection <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The connection request to handle.</param>
    void AcceptOrReject(ConnectionRequest request);

    /// <summary>
    /// Remove a peer that was previously disconnected.
    /// </summary>
    /// <param name="peer">The disconnected peer.</param>
    void RemoveDisconnected(NetPeer peer);
}
