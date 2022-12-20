using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.IoC.Abstractions;

namespace MultiplayerMapEditor.Editor.IoC;

internal sealed class HostContainerBuilder : IHostContainerBuilder
{
    public void ConfigureContainer(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IConnectionContainerBuilder, ConnectionContainerBuilder>();
    }
}
