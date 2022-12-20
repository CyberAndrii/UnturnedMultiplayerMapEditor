namespace MultiplayerMapEditor.Networking.Abstractions;

/// <summary>
/// Represents a connecting player.
/// </summary>
public class ConnectingPlayer
{
    public required NetPeer NetPeer { get; init; }

    /// <summary>
    /// A data that was sent by the peer on connection.
    /// </summary>
    public required ConnectionRequestData ConnectionRequestData { get; init; }
}
