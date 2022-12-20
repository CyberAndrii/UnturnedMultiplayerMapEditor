namespace MultiplayerMapEditor.Networking.Abstractions.Packets;

public interface INetPacketProcessor
{
    /// <summary>
    /// Registers a type of a nested property.
    /// </summary>
    /// <typeparam name="T">INetSerializable structure</typeparam>
    public void RegisterNestedType<T>()
        where T : struct, INetSerializable;

    /// <summary>
    /// Registers a type of a nested property.
    /// </summary>
    /// <param name="writeDelegate"></param>
    /// <param name="readDelegate"></param>
    public void RegisterNestedType<T>(Action<NetDataWriter, T> writeDelegate, Func<NetDataReader, T> readDelegate);

    /// <summary>
    /// Registers a type of a nested property.
    /// </summary>
    /// <typeparam name="T"><see cref="INetSerializable"/> class</typeparam>
    public void RegisterNestedType<T>(Func<T> constructor)
        where T : class, INetSerializable;

    /// <summary>
    /// Reads all available data from NetDataReader and calls OnReceive delegates.
    /// </summary>
    /// <param name="reader">NetDataReader to read packets from.</param>
    public void ReadAllPackets(NetDataReader reader);

    /// <summary>
    /// Reads all available data from NetDataReader and calls OnReceive delegates.
    /// </summary>
    /// <param name="reader">NetDataReader to read packets from.</param>
    /// <param name="userData">An argument that will be passed to OnReceivedEvent.</param>
    /// <exception cref="ParseException">Malformed packet.</exception>
    public void ReadAllPackets(NetDataReader reader, object userData);

    /// <summary>
    /// Reads one packet from NetDataReader and calls OnReceive delegate.
    /// </summary>
    /// <param name="reader">NetDataReader with packet</param>
    /// <exception cref="ParseException">Malformed packet</exception>
    public void ReadPacket(NetDataReader reader);

    public void Send<T>(NetPeer peer, T packet, DeliveryMethod options)
        where T : class, new();

    public void SendNetSerializable<T>(NetPeer peer, T packet, DeliveryMethod options)
        where T : INetSerializable;

    public void Send<T>(NetManager manager, T packet, DeliveryMethod options)
        where T : class, new();

    public void SendNetSerializable<T>(NetManager manager, T packet, DeliveryMethod options)
        where T : INetSerializable;

    public void Write<T>(NetDataWriter writer, T packet)
        where T : class, new();

    public void WriteNetSerializable<T>(NetDataWriter writer, T packet)
        where T : INetSerializable;

    public byte[] Write<T>(T packet)
        where T : class, new();

    public byte[] WriteNetSerializable<T>(T packet)
        where T : INetSerializable;

    /// <summary>
    /// Reads one packet from NetDataReader and calls OnReceive delegate.
    /// </summary>
    /// <param name="reader">NetDataReader with packet</param>
    /// <param name="userData">Argument that passed to OnReceivedEvent</param>
    /// <exception cref="ParseException">Malformed packet</exception>
    public void ReadPacket(NetDataReader reader, object userData);

    /// <summary>
    /// Registers and subscribes to packet receive event.
    /// </summary>
    /// <param name="onReceive">Event that will be called after packet is deserialized with ReadPacket method.</param>
    /// <param name="packetConstructor">Method that constructs the packet instead of the slow Activator.CreateInstance.</param>
    /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields.</exception>
    public void Subscribe<T>(Action<T> onReceive, Func<T> packetConstructor)
        where T : class, new();

    /// <summary>
    /// Registers and subscribes to packet receive event (with userData)
    /// </summary>
    /// <param name="onReceive">Event that will be called after packet is deserialized with ReadPacket method.</param>
    /// <param name="packetConstructor">Method that constructs the packet instead of the slow Activator.CreateInstance.</param>
    /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields.</exception>
    public void Subscribe<T, TUserData>(Action<T, TUserData> onReceive, Func<T> packetConstructor)
        where T : class, new();

    /// <summary>
    /// Registers and subscribes to packet receive event.
    /// This method will overwrite last received packet class on receive (less garbage).
    /// </summary>
    /// <param name="onReceive">Event that will be called after packet is deserialized with ReadPacket method.</param>
    /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields</exception>
    public void SubscribeReusable<T>(Action<T> onReceive)
        where T : class, new();

    /// <summary>
    /// Registers and subscribes to packet receive event.
    /// This method will overwrite last received packet class on receive (less garbage).
    /// </summary>
    /// <param name="onReceive">Event that will be called after packet is deserialized with ReadPacket method.</param>
    /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields.</exception>
    public void SubscribeReusable<T, TUserData>(Action<T, TUserData> onReceive)
        where T : class, new();

    public void SubscribeNetSerializable<T, TUserData>(Action<T, TUserData> onReceive, Func<T> packetConstructor)
        where T : INetSerializable;

    public void SubscribeNetSerializable<T>(Action<T> onReceive, Func<T> packetConstructor)
        where T : INetSerializable;

    public void SubscribeNetSerializable<T, TUserData>(Action<T, TUserData> onReceive)
        where T : INetSerializable, new();

    public void SubscribeNetSerializable<T>(Action<T> onReceive)
        where T : INetSerializable, new();

    /// <summary>
    /// Removes all subscriptions of a type.
    /// </summary>
    /// <typeparam name="T">The type of packet to unsubscribe.</typeparam>
    /// <returns>True if a subscription was found, otherwise false.</returns>
    public bool RemoveSubscription<T>();
}
