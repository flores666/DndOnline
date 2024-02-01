using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    [ForeignKey("Status")]
    public LobbyStatusType StatusId { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public Guid MasterId { get; set; }
    public List<User> Players { get; set; } 
    public List<Creature> Creatues { get; set; } 
    public List<Character> Characters { get; set; } 
    public List<Item> Items { get; set; } 
    public List<Map> Maps { get; set; }

    [JsonIgnore]
    public IEnumerable<CreaturePosition> CreatureLobby { get; set; } = new List<CreaturePosition>();
    
    [JsonIgnore]
    public IEnumerable<CharacterPosition> CharacterLobby { get; set; } = new List<CharacterPosition>();
    
    [JsonIgnore]
    public IEnumerable<LobbyMap> MapLobby { get; set; } = new List<LobbyMap>();
    
    [JsonIgnore]
    public IEnumerable<ItemPosition> ItemLobby { get; set; } = new List<ItemPosition>();

    public Lobby()
    {
        MaxPlayers = 6;
        StatusId = LobbyStatusType.Draft;
        Players = new List<User>();
        Creatues = new List<Creature>();
        Characters = new List<Character>();
        Items = new List<Item>();
        Maps = new List<Map>();
    }
}