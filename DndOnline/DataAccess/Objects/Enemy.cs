using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DndOnline.DataAccess.Objects;

public class Enemy
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required] 
    public string FullFilePath { get; set; }

    public IEnumerable<Lobby> Lobbies { get; set; } = new List<Lobby>();
    public IEnumerable<EnemyLobby> EnemyLobby { get; set; } = new List<EnemyLobby>();
}