using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class LobbyMap
{
    [ForeignKey("Map")]
    public Guid MapId { get; set; }
    public Map Map { get; set; }
    
    [ForeignKey("Lobby")]
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
}