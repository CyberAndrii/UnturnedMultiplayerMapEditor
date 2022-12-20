using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.Remove;

internal sealed class TellRemoveObjectPacket
{
    public NetId NetId { get; set; }
}

internal sealed class TellRemoveObjectRPC : IHostedService
{
    private readonly INetPacketProcessor _netPacketProcessor;
    private readonly ILevelObjectsManager _levelObjectsManager;

    public TellRemoveObjectRPC(
        INetPacketProcessor netPacketProcessor,
        ILevelObjectsManager levelObjectsManager)
    {
        _netPacketProcessor = netPacketProcessor;
        _levelObjectsManager = levelObjectsManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.SubscribeReusable<TellRemoveObjectPacket>(Handle);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _netPacketProcessor.RemoveSubscription<TellRemoveObjectPacket>();
        return Task.CompletedTask;
    }

    private void Handle(TellRemoveObjectPacket packet)
    {
        _levelObjectsManager.Remove(packet.NetId);
    }
}
