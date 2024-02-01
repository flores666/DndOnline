using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DndOnline.DataAccess.Objects;

public class Creature
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string RelativePath { get; set; }

    [JsonIgnore]
    public IEnumerable<Lobby> Lobbies { get; set; } = new List<Lobby>();
    
    [JsonIgnore]
    public IEnumerable<CreaturePosition> CreatureLobby { get; set; } = new List<CreaturePosition>();
}