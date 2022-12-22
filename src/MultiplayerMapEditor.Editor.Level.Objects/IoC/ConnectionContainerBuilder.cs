using Autofac;
using Microsoft.Extensions.Hosting;
using MultiplayerMapEditor.Editor.Level.Objects.Create;
using MultiplayerMapEditor.Editor.Level.Objects.Remove;
using MultiplayerMapEditor.Editor.Level.Objects.Transform;
using MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;
using MultiplayerMapEditor.IoC.Abstractions;
using EditorLevelObjectsUI = MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties.EditorLevelObjectsUI;

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
            builder.RegisterType<ClientsideNetObjectTransformer>().As<INetObjectTransformer>().SingleInstance();
            builder.RegisterType<ClientsideNetObjectPropertiesUpdater>()
                .As<INetObjectPropertiesUpdater>().SingleInstance();
            builder.RegisterType<TellCreateObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<TellRemoveObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<TellTransformObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<TellUpdateObjectPropertiesRPC>().As<IHostedService>().SingleInstance();
        }
        else
        {
            builder.RegisterType<ServersideNetObjectCreator>()
                .As<INetObjectCreator>().As<IServersideNetObjectCreator>().SingleInstance();
            builder.RegisterType<ServersideNetObjectRemover>().As<INetObjectRemover>().SingleInstance();
            builder.RegisterType<ServersideNetObjectTransformer>().As<INetObjectTransformer>().SingleInstance();
            builder.RegisterType<ServersideNetObjectPropertiesUpdater>()
                .As<INetObjectPropertiesUpdater>().SingleInstance();
            builder.RegisterType<AskCreateObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<AskRemoveObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<AskTransformObjectRPC>().As<IHostedService>().SingleInstance();
            builder.RegisterType<AskUpdateObjectPropertiesRPC>().As<IHostedService>().SingleInstance();
        }

        builder.RegisterType<LevelObjectsManager>().As<ILevelObjectsManager>().SingleInstance();
        builder.RegisterType<LevelObjectsReunReplacer>().As<IHostedService>().SingleInstance();
        builder.RegisterType<EditorLevelObjectsUI>().As<IHostedService>().SingleInstance();
    }
}
