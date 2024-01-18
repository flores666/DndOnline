using System.ComponentModel.DataAnnotations;

namespace DndOnline.DataAccess.Objects;

public class Map
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string FullFilePath { get; set; }

    public List<Lobby> Lobbies { get; set; }
    public IEnumerable<MapLobby> MapLobby { get; set; } = new List<MapLobby>();
}