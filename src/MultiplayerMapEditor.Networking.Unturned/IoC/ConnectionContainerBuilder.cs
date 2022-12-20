using Autofac;
using MultiplayerMapEditor.IoC.Abstractions;
using MultiplayerMapEditor.Networking.Abstractions.Packets;

namespace MultiplayerMapEditor.Networking.Unturned.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        builder.RegisterBuildCallback(lifetimeScope =>
        {
            var netPacketProcessor = lifetimeScope.Resolve<INetPacketProcessor>();

            netPacketProcessor.RegisterNetId();
        });
    }
}
