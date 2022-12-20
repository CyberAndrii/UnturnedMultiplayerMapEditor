using System.Reflection;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Abstractions;
using MultiplayerMapEditor.Abstractions.Level;
using MultiplayerMapEditor.Level;
using UnityEngine.SceneManagement;

namespace MultiplayerMapEditor.UI;

/// <summary>
/// Injects Host/Connect buttons into the MainMenu->Workshop->Edit UI.
/// </summary>
internal sealed class MenuWorkshopEditorUI : IHostedService
{
    private readonly ILogger _logger;
    private readonly IConnectionManager _connectionManager;
    private readonly ILevelManager _levelManager;
    private readonly MenuWorkshopEditorConnectUI _connectUI;

    private ISleekElement? _container;

    public MenuWorkshopEditorUI(
        ILogger<MenuWorkshopEditorUI> logger,
        IConnectionManager connectionManager,
        ILevelManager levelManager,
        MenuWorkshopEditorConnectUI connectUI)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _levelManager = levelManager;
        _connectUI = connectUI;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        return Task.CompletedTask;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Menu")
        {
            return;
        }

        InjectUIAsync().Forget();
    }

    private async UniTaskVoid InjectUIAsync()
    {
        await UniTask.NextFrame(); // wait until UI is created

        var container = (ISleekElement?)typeof(SDG.Unturned.MenuWorkshopEditorUI)
            .GetField("container", BindingFlags.NonPublic | BindingFlags.Static)?
            .GetValue(null);

        if (container == null)
        {
            _logger.LogError(
                "Failed to find MenuWorkshopEditorUI's container. " +
                "Most likely it was not yet created or the field was renamed"
            );

            return;
        }

        if (container == _container)
        {
            // already injected
            return;
        }

        _container = container;

        var hostButton = Glazier.Get().CreateButton();
        hostButton.positionOffset_X = -305;
        hostButton.positionOffset_Y = 610;
        hostButton.positionScale_X = 0.5f;
        hostButton.sizeOffset_X = 200;
        hostButton.sizeOffset_Y = 30;
        hostButton.text = "Host";
        hostButton.tooltipText = "Other players can join your session and edit the map together.";
        hostButton.onClickedButton += button => OnHostButtonClickedAsync().Forget();

        var connectButton = Glazier.Get().CreateButton();
        connectButton.positionOffset_X = -305;
        connectButton.positionOffset_Y = 650;
        connectButton.positionScale_X = 0.5f;
        connectButton.sizeOffset_X = 200;
        connectButton.sizeOffset_Y = 30;
        connectButton.text = "Connect";
        connectButton.tooltipText = "Connect to a multiplayer map editor.";
        connectButton.onClickedButton += button => OnConnectButtonClickedAsync().Forget();

        container.AddChild(hostButton);
        container.AddChild(connectButton);
    }

    private async UniTaskVoid OnHostButtonClickedAsync()
    {
        var levels = (LevelInfo[]?)typeof(SDG.Unturned.MenuWorkshopEditorUI)
            .GetField("levels", BindingFlags.NonPublic | BindingFlags.Static)?
            .GetValue(null);

        if (levels == null)
        {
            _logger.LogError("Failed to find MenuWorkshopEditorUI's levels");
            return;
        }

        foreach (var level in levels)
        {
            if (level.name != PlaySettings.editorMap || !level.isEditable)
            {
                continue;
            }

            // todo: configure options from UI
            await _connectionManager.HostAsync(new UnturnedLevelInfo(level), options => { });
            break;
        }
    }

    private async UniTaskVoid OnConnectButtonClickedAsync()
    {
        await _connectUI.OpenAsync();
    }
}
