using Autofac;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Editor.Level;
using MultiplayerMapEditor.Editor.Networking.EventHandlers;
using MultiplayerMapEditor.IoC.Abstractions;
using MultiplayerMapEditor.Networking.Abstractions.EventHandlers;

namespace MultiplayerMapEditor.Editor.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        if (isClient)
        {
            builder.RegisterType<DebugUI>().As<IHostedService>().SingleInstance();

            // Level
            builder.RegisterType<LoadLevelRPC>().As<IHostedService>().SingleInstance();
        }
        else
        {
            builder.RegisterDecorator<ServersidePeerConnectedEventHandler, IPeerConnectedEventHandler>();
        }
    }
}
