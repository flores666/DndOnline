using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class Location
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? Name { get; set; }
    public string Path { get; set; }
    // public IEnumerable<Lobby> Lobbies { get; set; } = new List<Lobby>();
}