using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    [ForeignKey("Type")]
    public Guid TypeId { get; set; }
    public EntityType Type { get; set; }
    public List<Lobby> Lobbies { get; set; } = new List<Lobby>();
    public IEnumerable<EntityLocation> EntityLocations { get; set; } = new List<EntityLocation>();
}