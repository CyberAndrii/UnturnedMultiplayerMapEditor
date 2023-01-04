using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiplayerMapEditor.IoC.Abstractions;
using Serilog;
using Serilog.Extensions.Logging;

namespace MultiplayerMapEditor.HostBuilder;

/// <summary>
/// Starts/stop the application's host.
/// </summary>
public sealed class Bootstrapper : IBootstrapper, IAsyncDisposable
{
    private readonly string _workingDirectory;
    private readonly int _mainThreadId;
    private IHost? _host;

    public Bootstrapper(string workingDirectory, int mainThreadId)
    {
        _workingDirectory = workingDirectory;
        _mainThreadId = mainThreadId;
    }

    /// <summary>
    /// Gets <see cref="IServiceProvider"/> of current running host, or null when is not running.
    /// </summary>
    public IServiceProvider? Services => _host?.Services;

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        if (_host != null)
        {
            throw new InvalidOperationException("Host is already created");
        }

        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(ConfigureHostConfiguration)
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging((context, builder) =>
            {
                builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                builder.ClearProviders();
                builder.AddSerilog();
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<UniTaskPlayerLoopOptions>(options => options.MainThreadId = _mainThreadId);
                services.AddHostedService<UniTaskPlayerLoop>(); // must be first, before any UniTask use

                // todo: find with reflection
                // Dynamic assembly loading is not working, so hardcode
                var hostContainerBuilders = new[]
                {
                    typeof(MultiplayerMapEditor.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Editor.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Editor.Level.Objects.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Editor.Terrain.Heightmap.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Networking.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Networking.System.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Networking.UnityEngine.IoC.HostContainerBuilder),
                    typeof(MultiplayerMapEditor.Networking.Unturned.IoC.HostContainerBuilder),
                };

                foreach (var hostContainerBuilder in hostContainerBuilders)
                {
                    var instance = (IHostContainerBuilder)Activator.CreateInstance(hostContainerBuilder);
                    instance.ConfigureContainer(context, services);
                }
            })
            .UseSerilog((context, services, config) =>
            {
                config
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.WithDemystifiedStackTraces()
                    .WriteTo.Async(asyncConfig =>
                    {
                        asyncConfig.Providers(new LoggerProviderCollection())
                            .WriteTo.File(GetPathForNewLogFile(),
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] " +
                                                "{Message:lj}{NewLine}{Exception}"
                            );
                    });
            });

        try
        {
            _host = await hostBuilder.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _host = null;
            throw;
        }
    }

    private void ConfigureHostConfiguration(IConfigurationBuilder builder)
    {
        builder
            .SetBasePath(_workingDirectory)
            .AddJsonFile(
                "config.json",
                optional: false,
                reloadOnChange: true
            )
            .AddEnvironmentVariables();
    }

    private void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder
            .SetBasePath(_workingDirectory)
            .AddJsonFile(
                "config.json",
                optional: false,
                reloadOnChange: true
            )
            .AddJsonFile(
                $"config.{context.HostingEnvironment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true
            )
            .AddEnvironmentVariables();
    }

    private string GetPathForNewLogFile()
    {
        var logPath = Path.Combine(_workingDirectory, "logs", "MultiplayerMapEditor.log");

        if (File.Exists(logPath))
        {
            var prevLogPath = Path.Combine(_workingDirectory, "logs", "MultiplayerMapEditor-Prev.log");

            File.Delete(prevLogPath);
            File.Move(logPath, prevLogPath);
        }

        return logPath;
    }

    public async UniTask StopAsync(CancellationToken cancellationToken)
    {
        if (_host == null)
        {
            return;
        }

        try
        {
            await _host.StopAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _host.Dispose();
            _host = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync(CancellationToken.None);
    }
}
