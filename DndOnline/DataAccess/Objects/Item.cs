using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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

    public Guid UserId { get; set; }

    public List<Lobby> Lobbies { get; set; }
    
    [JsonIgnore]
    public IEnumerable<ItemPosition> ItemLobby { get; set; } = new List<ItemPosition>();
}