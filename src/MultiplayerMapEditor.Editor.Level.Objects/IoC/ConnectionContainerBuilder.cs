using Autofac;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Editor.Level.Objects.Create;
using MultiplayerMapEditor.Editor.Level.Objects.Remove;
using MultiplayerMapEditor.IoC.Abstractions;

namespace MultiplayerMapEditor.Editor.Level.Objects.IoC;

internal sealed class ConnectionContainerBuilder : IConnectionContainerBuilder
{
    public void ConfigureContainer(ContainerBuilder builder, bool isClient)
    {
        if (isClient)
        {
            builder.RegisterType<ClientsideNetObjectCreator>()
                .As<INetObjectCreator>().As<IClientsideNetObjectCreator>().SingleInstance();
            builder.RegisterType<ClientsideNetObjectRemover>().As<INetObjectRemover>().SingleInstance();
            builder.RegisterType<TellCreateObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<TellRemoveObjectRPC>().As<IHostedService>().SingleInstance();
        }
        else
        {
            builder.RegisterType<ServersideNetObjectCreator>()
                .As<INetObjectCreator>().As<IServersideNetObjectCreator>().SingleInstance();
            builder.RegisterType<ServersideNetObjectRemover>().As<INetObjectRemover>().SingleInstance();
            builder.RegisterType<AskCreateObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<AskRemoveObjectRPC>().As<IHostedService>().SingleInstance();
        }

        builder.RegisterType<LevelObjectsManager>().As<ILevelObjectsManager>().SingleInstance();
        builder.RegisterType<LevelObjectsReunReplacer>().As<IHostedService>().SingleInstance();
    }
}
