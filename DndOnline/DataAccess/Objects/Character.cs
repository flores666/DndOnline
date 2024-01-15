using System.ComponentModel.DataAnnotations;

namespace DndOnline.DataAccess.Objects;

public class Character
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string FullFilePath { get; set; }

    public List<Lobby> Lobbies { get; set; }
}