using System.Net;

namespace MultiplayerMapEditor.Networking.Abstractions;

/// <summary>
/// The base interface of the the network manager.
/// </summary>
public interface INetManager
{
    /// <summary>
    /// Determines whether socket listening and update thread is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets low level networking manager.
    /// </summary>
    NetManager NetManager { get; }
}

public interface IClientsideNetManager : INetManager
{
    /// <summary>
    /// Determines whether the client is connected to a server.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// The peer of the server connected to.
    /// Returns null if not yet connected.
    /// </summary>
    NetPeer? ServerPeer { get; }

    /// <summary>
    /// Connects to a server.
    /// </summary>
    /// <param name="ipEndPoint">The address to connect to.</param>
    /// <param name="data">Request data to send to the server along with connection request.</param>
    void Connect(IPEndPoint ipEndPoint, ConnectionRequestData data);
}

public interface IServersideNetManager : INetManager
{
    /// <summary>
    /// Begins hosting a server.
    /// </summary>
    void Host();

    /// <summary>
    /// Disconnects the <paramref name="peer"/> from the server.
    /// </summary>
    /// <param name="peer">The peer to disconnect.</param>
    /// <param name="disconnectReason">The reason of the disconnect.</param>
    void Disconnect(NetPeer peer, Packets.DisconnectReason disconnectReason);
}
