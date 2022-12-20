using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

internal sealed class AskRemoveObjectPacket
{
    public NetId NetId { get; set; }
}

internal sealed class AskRemoveObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly INetObjectRemover _netObjectRemover;

    public AskRemoveObjectRPC(
        INetPacketProcessor netPacketProcessor,
        INetObjectRemover netObjectRemover)
    {
        _netPacketProcessor = netPacketProcessor;
        _netObjectRemover = netObjectRemover;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<AskRemoveObjectPacket, NetPeer>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<AskRemoveObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(AskRemoveObjectPacket packet, NetPeer peer)
    {
        _netObjectRemover.Remove(packet.NetId);
    }
}
