using System.Net;
using MultiplayerMapEditor.Abstractions.Level;
using MultiplayerMapEditor.Networking.Abstractions;

namespace MultiplayerMapEditor.Abstractions;

/// <summary>
/// The main manager for managing connections to the server or hosting it itself.
/// </summary>
public interface IConnectionManager
{
    /// <summary>
    /// Starts hosting a server.
    /// </summary>
    /// <param name="levelInfo">The level to host on.</param>
    /// <param name="configureAction">Options configurator.</param>
    /// <returns></returns>
    UniTask HostAsync(ILevelInfo levelInfo, Action<ServerOptions> configureAction);

    /// <summary>
    /// Connects to the server at the given address.
    /// </summary>
    /// <param name="ipEndPoint">The address to connect to.</param>
    /// <param name="configureAction">Options configurator.</param>
    /// <returns></returns>
    UniTask ConnectAsync(IPEndPoint ipEndPoint, Action<ClientOptions> configureAction);

    /// <summary>
    /// Stops hosting (if host) or disconnects from the server (if client).
    /// </summary>
    /// <param name="disconnectReason">The disconnect reason. Empty if none.</param>
    /// <returns></returns>
    UniTask StopAsync(string disconnectReason);
}
