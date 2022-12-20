using System.Net;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Abstractions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerMapEditor.UI;

/// <summary>
/// Creates the MainMenu->Workshop->Edit->Connect window and manages it's state.
/// </summary>
internal sealed class MenuWorkshopEditorConnectUI : IHostedService, IDisposable
{
    private readonly IConnectionManager _connectionManager;
    private CancellationTokenSource? _cts;

    private bool _active;
    private SleekFullscreenBox? _container;
    private SleekButtonIcon? _backButton;
    private ISleekField? _ipField;
    private ISleekUInt16Field? _portField;
    private ISleekField? _passwordField;
    private SleekButtonIcon? _connectButton;

    public MenuWorkshopEditorConnectUI(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name != "Menu")
        {
            return;
        }

        // Container is destroyed on scene unload,
        // so we set it to null to create a new one later
        _container = null;
        _active = false;

        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private async UniTask CreateWindowAsync()
    {
        await UniTask.NextFrame(); // wait until UI is created

        if (_container != null)
        {
            // already created
            return;
        }

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            throw new InvalidOperationException("Current scene is not the Menu");
        }

        var bundle = Bundles.getBundle("/Bundles/Textures/Menu/Icons/Play/MenuPlayConnect/MenuPlayConnect.unity3d");
        using var __ = Defer(() => bundle.unload());

        _active = false;
        _container = new SleekFullscreenBox
        {
            positionOffset_X = 10,
            positionOffset_Y = 10,
            positionScale_Y = 1f,
            sizeOffset_X = -20,
            sizeOffset_Y = -20,
            sizeScale_X = 1f,
            sizeScale_Y = 1f
        };

        _ipField = Glazier.Get().CreateStringField();
        _ipField.positionOffset_X = -100;
        _ipField.positionOffset_Y = -75;
        _ipField.positionScale_X = 0.5f;
        _ipField.positionScale_Y = 0.5f;
        _ipField.sizeOffset_X = 200;
        _ipField.sizeOffset_Y = 30;
        _ipField.maxLength = 64;
        _ipField.addLabel("IP", ESleekSide.RIGHT);
        _ipField.text = PlaySettings.connectIP;

        _portField = Glazier.Get().CreateUInt16Field();
        _portField.positionOffset_X = -100;
        _portField.positionOffset_Y = -35;
        _portField.positionScale_X = 0.5f;
        _portField.positionScale_Y = 0.5f;
        _portField.sizeOffset_X = 200;
        _portField.sizeOffset_Y = 30;
        _portField.addLabel("Port", ESleekSide.RIGHT);
        _portField.state = PlaySettings.connectPort;

        _passwordField = Glazier.Get().CreateStringField();
        _passwordField.positionOffset_X = -100;
        _passwordField.positionOffset_Y = 5;
        _passwordField.positionScale_X = 0.5f;
        _passwordField.positionScale_Y = 0.5f;
        _passwordField.sizeOffset_X = 200;
        _passwordField.sizeOffset_Y = 30;
        _passwordField.addLabel("Password", ESleekSide.RIGHT);
        _passwordField.replace = '#';
        _passwordField.maxLength = 0;
        _passwordField.text = PlaySettings.connectPassword;

        _connectButton = new SleekButtonIcon(bundle.load<Texture2D>("Connect"))
        {
            positionOffset_X = -100,
            positionOffset_Y = 45,
            positionScale_X = 0.5f,
            positionScale_Y = 0.5f,
            sizeOffset_X = 200,
            sizeOffset_Y = 30,
            text = "Connect",
            tooltip = "Connect to the server",
            iconColor = ESleekTint.FOREGROUND
        };
        _connectButton.onClickedButton += button => OnConnectButtonClickedAsync().Forget();

        _backButton = new SleekButtonIcon(MenuDashboardUI.icons.load<Texture2D>("Exit"))
        {
            positionOffset_Y = -50,
            positionScale_Y = 1f,
            sizeOffset_X = 200,
            sizeOffset_Y = 50,
            text = "Back",
            tooltip = "Back",
            fontSize = ESleekFontSize.Medium,
            iconColor = ESleekTint.FOREGROUND
        };
        _backButton.onClickedButton += button => OnBackButtonClickedAsync().Forget();

        MenuUI.container.AddChild(_container);
        _container.AddChild(_ipField);
        _container.AddChild(_portField);
        _container.AddChild(_passwordField);
        _container.AddChild(_connectButton);
        _container.AddChild(_backButton);
    }

    private async UniTaskVoid OnConnectButtonClickedAsync()
    {
        IPEndPoint ipEndPoint;

        try
        {
            var ipAddress = IPAddress.Parse(_ipField!.text);
            ipEndPoint = new IPEndPoint(ipAddress, _portField!.state);

            SavePlayerSettings();
        }
        catch
        {
            await UniTask.SwitchToMainThread();

            try
            {
                await CloseAsync();
            }
            finally
            {
                MenuUI.openAlert("An invalid IP address or port was specified.");
            }

            return;
        }

        await _connectionManager.ConnectAsync(ipEndPoint, configureAction: _ => { });
    }

    private async UniTaskVoid OnBackButtonClickedAsync()
    {
        await CloseAsync();
    }

    private void SavePlayerSettings()
    {
        PlaySettings.connectIP = _ipField?.text ?? PlaySettings.connectIP;
        PlaySettings.connectPort = _portField?.state ?? PlaySettings.connectPort;
        PlaySettings.connectPassword = _passwordField?.text ?? PlaySettings.connectPassword;
    }

    public async UniTask OpenAsync()
    {
        await UniTask.SwitchToMainThread();

        if (_active)
        {
            return;
        }

        if (_container == null)
        {
            await CreateWindowAsync();
        }

        _active = true;
        _container!.AnimateIntoView();

        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        _cts = new CancellationTokenSource();

        EscapeLoopAsync(_cts.Token).Forget();

        SDG.Unturned.MenuWorkshopEditorUI.close();
    }

    public async UniTask CloseAsync()
    {
        await UniTask.SwitchToMainThread();

        if (!_active)
        {
            return;
        }

        _active = false;

        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        if (_container != null)
        {
            _container.AnimateOutOfView(0f, 1f);
            _container.isVisible = false;
        }

        SDG.Unturned.MenuWorkshopEditorUI.open();
    }

    /// <summary>
    /// Opens previous window when the Escape key is pressed.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async UniTaskVoid EscapeLoopAsync(CancellationToken cancellationToken)
    {
        // Runs on EarlyUpdate before Unturned's code do.
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate(PlayerLoopTiming.EarlyUpdate))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!_active)
            {
                continue;
            }

            if (InputEx.ConsumeKeyDown(KeyCode.Escape))
            {
                await CloseAsync();
            }
        }
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}
