using Autofac;
using MultiplayerMapEditor.Editor.Terrain.Heightmap.Update;
using MultiplayerMapEditor.IoC.Abstractions;

namespace MultiplayerMapEditor.Editor.Terrain.Heightmap.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        if (isClient)
        {
            builder.RegisterType<ClientsideNetHeightmapManager>().As<INetHeightmapManager>().SingleInstance();
            builder.RegisterType<TellUpdateHeightmapRPC>().As<IHostedService>().SingleInstance();
        }
        else
        {
            builder.RegisterType<ServersideNetHeightmapManager>().As<INetHeightmapManager>().SingleInstance();
            builder.RegisterType<AskUpdateHeightmapRPC>().As<IHostedService>().SingleInstance();
        }

        builder.RegisterType<LandscapeInterceptor>().As<IHostedService>().SingleInstance();
        builder.RegisterType<LandscapeHeightmapTransactionInterceptor>().As<IHostedService>().SingleInstance();
    }
}
