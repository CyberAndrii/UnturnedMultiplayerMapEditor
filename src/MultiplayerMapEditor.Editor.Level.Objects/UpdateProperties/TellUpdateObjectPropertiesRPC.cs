using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal sealed class TellUpdateObjectPropertiesPacket
{
    public NetId NetId { get; set; }
    public Guid MaterialOverride { get; set; }
    public int MaterialIndexOverride { get; set; }
}

internal sealed class TellUpdateObjectPropertiesRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public TellUpdateObjectPropertiesRPC(
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<TellUpdateObjectPropertiesPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<TellUpdateObjectPropertiesPacket>();
        return Task.CompletedTask;
    }

    private void Handle(TellUpdateObjectPropertiesPacket packet, NetPeer peer)
    {
        _levelObjectsManager.UpdateObjectProperties(
            packet.NetId,
            packet.MaterialOverride,
            packet.MaterialIndexOverride
        );
    }
}
