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
    public IEnumerable<Entity> Entities { get; set; } = new List<Entity>();
    public IEnumerable<Map> Maps { get; set; } = new List<Map>();
    public IEnumerable<Location> EntityLocations { get; set; } = new List<Location>();
    public IEnumerable<LobbyMap> LobbyMaps { get; set; } = new List<LobbyMap>();

    public Lobby()
    {
        MaxPlayers = 6;
        StatusId = LobbyStatusType.Draft;
        Players = new List<User>();
    }
}