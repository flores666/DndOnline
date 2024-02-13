using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("Picture")]
    public Guid PictureId { get; set; }
    public Picture Picture { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
    public IEnumerable<Location> Locations { get; set; } = new List<Location>();
    public IEnumerable<Lobby> Lobbies { get; set; } = new List<Lobby>();
}