using System.Net;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MultiplayerMapEditor.Abstractions;
using MultiplayerMapEditor.Abstractions.Level;
using MultiplayerMapEditor.IoC.Abstractions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerMapEditor;

internal sealed class ConnectionManager : IConnectionManager, IAsyncDisposable
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILevelManager _levelManager;
    private readonly ILogger<ConnectionManager> _logger;
    private ILifetimeScope? _childLifetimeScope;
    private IHostedService[]? _childHostedServices;

    private bool _quitNextTime;

    public ConnectionManager(
        ILifetimeScope lifetimeScope,
        ILevelManager levelManager,
        ILogger<ConnectionManager> logger)
    {
        _lifetimeScope = lifetimeScope;
        _levelManager = levelManager;
        _logger = logger;

        Application.wantsToQuit += HandleWantsToQuit;
    }

    public async UniTask HostAsync(ILevelInfo levelInfo, Action<ServerOptions> configurator)
    {
        try
        {
            AssertChildLifetimeScopeDoesNotExist();
            await InternalHostAsync(levelInfo, configurator);
        }
        catch
        {
            await StopAsync("An error occurred while hosting. See logs for more details.");
            throw;
        }
    }

    private async UniTask InternalHostAsync(ILevelInfo levelInfo, Action<ServerOptions> configureAction)
    {
        await UniTask.SwitchToTaskPool();

        _childLifetimeScope = _lifetimeScope.BeginLifetimeScope(
            builder => BuildConnectionContainer(builder, isClient: false)
        );

        IServersideNetManager netManager;

        try
        {
            var options = _childLifetimeScope.Resolve<IOptions<ServerOptions>>();
            options.Value.StopAsyncFunc = async disconnectReason => await StopAsync(disconnectReason);
            configureAction(options.Value);

            await StartHostedServicesAsync(CancellationToken.None);

            netManager = _childLifetimeScope.Resolve<IServersideNetManager>();
        }
        catch (Exception ex1)
        {
            try
            {
                await _childLifetimeScope.DisposeAsync();
                _childLifetimeScope = null;
            }
            catch (Exception ex2)
            {
                throw new AggregateException(ex1, ex2);
            }

            throw;
        }

        await UniTask.SwitchToMainThread();

        _levelManager.OnLevelExit += HandleLevelExit;

        _levelManager.LoadLevel(levelInfo);

        netManager.Host();
    }

    public async UniTask ConnectAsync(IPEndPoint ipEndPoint, Action<ClientOptions> configureAction)
    {
        try
        {
            AssertChildLifetimeScopeDoesNotExist();
            await InternalConnectAsync(ipEndPoint, configureAction);
        }
        catch
        {
            await StopAsync("An error occurred while connecting to the server. See logs for more details.");
            throw;
        }
    }

    private async UniTask InternalConnectAsync(IPEndPoint ipEndPoint, Action<ClientOptions> configureAction)
    {
        await UniTask.SwitchToTaskPool();

        _childLifetimeScope = _lifetimeScope.BeginLifetimeScope(
            builder => BuildConnectionContainer(builder, isClient: true)
        );

        IClientsideNetManager netManager;

        try
        {
            var options = _childLifetimeScope.Resolve<IOptions<ClientOptions>>();
            options.Value.StopAsyncFunc = async disconnectReason => await StopAsync(disconnectReason);
            configureAction(options.Value);

            await StartHostedServicesAsync(CancellationToken.None);

            netManager = _childLifetimeScope.Resolve<IClientsideNetManager>();
        }
        catch (Exception ex1)
        {
            try
            {
                await _childLifetimeScope.DisposeAsync();
                _childLifetimeScope = null;
            }
            catch (Exception ex2)
            {
                throw new AggregateException(ex1, ex2);
            }

            throw;
        }

        await UniTask.SwitchToMainThread();

        _levelManager.OnLevelExit += HandleLevelExit;

        MenuUI.openAlert("Connecting...", canBeDismissed: false);

        var data = new ConnectionRequestData
        {
            SteamId = Provider.client.m_SteamID, //
            Username = Provider.clientName,
        };

        netManager.Connect(ipEndPoint, data);

        // level is loaded after successful connection
    }

    private void BuildConnectionContainer(ContainerBuilder containerBuilder, bool isClient)
    {
        var builders = _lifetimeScope.Resolve<IEnumerable<IConnectionContainerBuilder>>();

        foreach (var builder in builders)
        {
            builder.ConfigureContainer(containerBuilder, isClient);
        }
    }

    public async UniTask StopAsync(string disconnectReason)
    {
        await UniTask.SwitchToTaskPool();

        if (_childLifetimeScope == null)
        {
            return;
        }

        try
        {
            try
            {
                await StopHostedServicesAsync(CancellationToken.None);
            }
            finally
            {
                var scope = _childLifetimeScope;
                _childLifetimeScope = null;
                await scope.ConfigureAwait(false).DisposeAsync();
            }
        }
        finally
        {
            await UniTask.SwitchToMainThread();

            _levelManager.OnLevelExit -= HandleLevelExit;
            Application.wantsToQuit -= HandleWantsToQuit;

            var isInMainMenu = SceneManager.GetActiveScene().name == "Menu";

            if (isInMainMenu)
            {
                if (disconnectReason != "")
                {
                    MenuUI.openAlert(disconnectReason, canBeDismissed: true);
                }
            }
            else
            {
                Provider.resetConnectionFailure();

                Provider.connectionFailureInfo = disconnectReason == ""
                    ? ESteamConnectionFailureInfo.NONE
                    : ESteamConnectionFailureInfo.CUSTOM;

                Provider.connectionFailureReason = disconnectReason;

                _levelManager.Exit();
            }
        }
    }

    private void HandleLevelExit()
    {
        StopAsync("").Forget();
    }

    /// <summary>
    /// Cancels quit, calls <see cref="StopAsync"/>, then requests quit again.
    /// This prevents a deadlock when calling UniTask.SwitchToMainThread() on quit.
    /// It deadlocks because no more Update() events will be run.
    /// </summary>
    /// <returns>Returns true when can quit, otherwise false.</returns>
    private bool HandleWantsToQuit()
    {
        if (_quitNextTime)
        {
            return true;
        }

        StopAsync("Application is quitting").ContinueWith(() =>
        {
            _quitNextTime = true;
            Application.Quit();
        }).Forget();

        return _quitNextTime;
    }

    private async UniTask StartHostedServicesAsync(CancellationToken cancellationToken)
    {
        var parentHostedServices = _lifetimeScope.Resolve<IEnumerable<IHostedService>>();
        var childHostedServices = _childLifetimeScope!.Resolve<IEnumerable<IHostedService>>();

        _childHostedServices = childHostedServices
            .Where(s => !parentHostedServices.Contains(s))
            .ToArray();

        foreach (var hostedService in _childHostedServices)
        {
            _logger.LogTrace("Starting hosted service {HostedServiceType}", hostedService.GetType().FullName);
            await hostedService.StartAsync(cancellationToken);
        }
    }

    private async UniTask StopHostedServicesAsync(CancellationToken cancellationToken)
    {
        // In case if an error occurred during startup and services were not started
        if (_childHostedServices == null)
        {
            return;
        }

        List<Exception>? exceptions = null;

        foreach (var hostedService in _childHostedServices.Reverse())
        {
            try
            {
                _logger.LogTrace("Stopping hosted service {HostedServiceType}", hostedService.GetType().FullName);
                await hostedService.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                (exceptions ??= new List<Exception>()).Add(ex);
            }
        }

        if (exceptions?.Count > 0)
        {
            throw new AggregateException("One or more hosted services failed to stop.", exceptions);
        }

        _childHostedServices = null;
    }

    private void AssertChildLifetimeScopeDoesNotExist()
    {
        if (_childLifetimeScope != null)
        {
            throw new InvalidOperationException("Child lifetime scope already exists.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync(disconnectReason: "");
    }
}
