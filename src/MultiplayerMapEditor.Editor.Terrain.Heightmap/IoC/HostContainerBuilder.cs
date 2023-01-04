using Microsoft.Extensions.DependencyInjection;
using MultiplayerMapEditor.IoC.Abstractions;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap.IoC;

internal sealed class HostContainerBuilder : IHostContainerBuilder
{
    public void ConfigureContainer(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IConnectionContainerBuilder, ConnectionContainerBuilder>();
    }
}
