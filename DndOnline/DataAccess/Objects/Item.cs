using System.ComponentModel.DataAnnotations;

namespace DndOnline.DataAccess.Objects;

public class Item
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string RelativePath { get; set; }

    public List<Lobby> Lobbies { get; set; }
    public IEnumerable<ItemLobby> ItemLobby { get; set; } = new List<ItemLobby>();
}