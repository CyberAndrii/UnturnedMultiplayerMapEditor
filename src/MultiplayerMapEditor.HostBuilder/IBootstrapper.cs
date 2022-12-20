namespace MultiplayerMapEditor.HostBuilder;

/// <summary>
/// Manages lifetime if the host.
/// </summary>
public interface IBootstrapper
{
    /// <summary>
    /// Gets services of the built container.
    /// Null if not yet built.
    /// </summary>
    IServiceProvider? Services { get; }

    UniTask StartAsync(CancellationToken cancellationToken);

    UniTask StopAsync(CancellationToken cancellationToken);
}
