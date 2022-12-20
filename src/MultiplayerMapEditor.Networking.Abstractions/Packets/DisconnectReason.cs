namespace MultiplayerMapEditor.Networking.Abstractions.Packets;

public struct DisconnectReason : INetSerializable
{
    public DisconnectReason()
    {
        Message = "";
    }

    public string Message { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Message);
    }

    public void Deserialize(NetDataReader reader)
    {
        Message = reader.GetString();
    }
}
