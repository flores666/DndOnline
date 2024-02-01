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

    public IEnumerable<CreaturePosition> CreaturePositions { get; set; } = new List<CreaturePosition>();
    public IEnumerable<CharacterPosition> CharacterPositions { get; set; } = new List<CharacterPosition>();
    public IEnumerable<ItemPosition> ItemPositions { get; set; } = new List<ItemPosition>();
    public IEnumerable<LobbyMap> LobbyMaps { get; set; } = new List<LobbyMap>();

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