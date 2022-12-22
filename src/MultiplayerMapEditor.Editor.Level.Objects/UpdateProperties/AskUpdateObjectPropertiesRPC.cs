using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal sealed class AskUpdateObjectPropertiesPacket
{
    public NetId NetId { get; set; }
    public Guid MaterialOverride { get; set; }
    public int MaterialIndexOverride { get; set; }
}

internal sealed class AskUpdateObjectPropertiesRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly INetObjectPropertiesUpdater _netObjectPropertiesUpdater;

    public AskUpdateObjectPropertiesRPC(
        INetPacketProcessor netPacketProcessor,
        INetObjectPropertiesUpdater netObjectPropertiesUpdater)
    {
        _netPacketProcessor = netPacketProcessor;
        _netObjectPropertiesUpdater = netObjectPropertiesUpdater;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<AskUpdateObjectPropertiesPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<AskUpdateObjectPropertiesPacket>();
        return Task.CompletedTask;
    }

    private void Handle(AskUpdateObjectPropertiesPacket packet, NetPeer peer)
    {
        _netObjectPropertiesUpdater.UpdateObjectProperties(
            packet.NetId,
            packet.MaterialOverride,
            packet.MaterialIndexOverride
        );
    }
}
