namespace MultiplayerMapEditor.Networking.Abstractions.Packets;

public class ConnectionRequestData : INetSerializable
{
    public ulong SteamId { get; set; }

    public string Username { get; set; } = null!;

    //public string ModuleVersion { get; set; } = null!;

    //public byte[] ModuleHash { get; set; } = null!;


    public void Serialize(NetDataWriter writer)
    {
        writer.Put(SteamId);
        writer.Put(Username);
    }

    public void Deserialize(NetDataReader reader)
    {
        SteamId = reader.GetULong();
        Username = reader.GetString();
    }
}
