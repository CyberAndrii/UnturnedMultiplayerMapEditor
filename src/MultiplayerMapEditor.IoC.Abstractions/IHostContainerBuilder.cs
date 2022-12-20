using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.IoC.Abstractions;

public interface IHostContainerBuilder
{
    void ConfigureContainer(HostBuilderContext context, IServiceCollection services);
}
