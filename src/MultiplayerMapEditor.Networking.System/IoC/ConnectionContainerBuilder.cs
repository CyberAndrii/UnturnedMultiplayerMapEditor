using Autofac;
using MultiplayerMapEditor.IoC.Abstractions;
using MultiplayerMapEditor.Networking.Abstractions.Packets;

namespace MultiplayerMapEditor.Networking.System.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        builder.RegisterBuildCallback(lifetimeScope =>
        {
            var netPacketProcessor = lifetimeScope.Resolve<INetPacketProcessor>();

            netPacketProcessor.RegisterGuid();
            netPacketProcessor.RegisterNullable((writer, value) => writer.Put(value), reader => reader.GetGuid());
            netPacketProcessor.RegisterNullable((writer, value) => writer.Put(value), reader => reader.GetInt());
        });
    }
}
