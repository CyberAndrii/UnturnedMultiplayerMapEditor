namespace MultiplayerMapEditor.IoC.Abstractions;

public interface IConnectionContainerBuilder
{
    void ConfigureContainer(ContainerBuilder builder, bool isClient);
}
