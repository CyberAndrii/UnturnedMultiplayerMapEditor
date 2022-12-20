using Autofac;
using MultiplayerMapEditor.IoC.Abstractions;
using MultiplayerMapEditor.Networking.EventHandlers;

namespace MultiplayerMapEditor.Networking.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        builder.RegisterTypeTernary<ClientsideNetManager, ServersideNetManager>(isClient)
            .AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<NetPacketProcessor>().As<INetPacketProcessor>().SingleInstance();

        // Event handlers
        builder.RegisterType<NetEventListenerAdapter>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<PeerConnectedEventHandler>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterTypeTernary<ClientsidePeerDisconnectedEventHandler, ServersidePeerDisconnectedEventHandler>
            (isClient).AsImplementedInterfaces().SingleInstance();
        builder.RegisterTypeTernary<ClientsideNetworkErrorEventHandler, ServersideNetworkErrorEventHandler>(isClient)
            .AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<NetworkReceiveEventHandler>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<NetworkReceiveUnconnectedEventHandler>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<NetworkLatencyUpdateEventHandler>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterTypeTernary<ClientsideConnectionRequestEventHandler, ServersideConnectionRequestEventHandler>
            (isClient).AsImplementedInterfaces().SingleInstance();

        if (isClient)
        {
        }
        else
        {
            builder.RegisterType<ClientRegistry>().As<IClientRegistry>().SingleInstance();
        }
    }
}
