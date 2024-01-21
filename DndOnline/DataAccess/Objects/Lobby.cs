namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public string Master { get; set; }
    public List<User> Players { get; set; } 
    public List<Creature> Creatues { get; set; } 
    public List<Character> Characters { get; set; } 
    public List<Item> Items { get; set; } 
    public List<Map> Maps { get; set; }

    public IEnumerable<CreatureLobby> CreatureLobby { get; set; } = new List<CreatureLobby>();
    public IEnumerable<CharacterLobby> CharacterLobby { get; set; } = new List<CharacterLobby>();
    public IEnumerable<MapLobby> MapLobby { get; set; } = new List<MapLobby>();
    public IEnumerable<ItemLobby> ItemLobby { get; set; } = new List<ItemLobby>();

    public Lobby()
    {
        MaxPlayers = 6;
        Status = new LobbyStatus();
        Players = new List<User>();
        Creatues = new List<Creature>();
        Characters = new List<Character>();
        Items = new List<Item>();
        Maps = new List<Map>();
    }
}