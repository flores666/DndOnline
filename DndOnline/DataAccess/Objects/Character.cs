using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DndOnline.DataAccess.Objects;

public class Character
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string RelativePath { get; set; }
    
    public Guid UserId { get; set; }

    public List<Lobby> Lobbies { get; set; }
    
    [JsonIgnore]
    public IEnumerable<CharacterPosition> CharacterLobby { get; set; } = new List<CharacterPosition>();
}