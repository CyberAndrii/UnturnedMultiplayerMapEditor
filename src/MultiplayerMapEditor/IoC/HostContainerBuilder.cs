using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Abstractions;
using MultiplayerMapEditor.Abstractions.Level;
using MultiplayerMapEditor.IoC.Abstractions;
using MultiplayerMapEditor.Level;
using MultiplayerMapEditor.UI;

namespace MultiplayerMapEditor.IoC;

internal sealed class HostContainerBuilder : IHostContainerBuilder
{
    public void ConfigureContainer(HostBuilderContext context, IServiceCollection services)
    {
        // Level
        services.AddSingleton<ILevelManager, UnturnedLevelManager>();

        // UI
        services.AddHostedService<MultiplayerMapEditor.UI.MenuWorkshopEditorUI>();
        services.AddSingleton<MenuWorkshopEditorConnectUI>();
        services.AddHostedService<MenuWorkshopEditorConnectUI>(
            serviceProvider => serviceProvider.GetRequiredService<MenuWorkshopEditorConnectUI>()
        );

        // Connection
        services.AddSingleton<IConnectionManager, ConnectionManager>();
    }
}
