using System.ComponentModel.DataAnnotations;

namespace DndOnline.Models;

public class LobbyFormViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public int MaxPlayers { get; set; } = 6;
    
    [Required]
    public string Master { get; set; }
}