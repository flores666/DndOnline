using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class EntitySubType
{
    [Key] 
    public Guid Id { get; set; }
    [ForeignKey("Type")]
    public Guid TypeId { get; set; }
    public EntityType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    private IEnumerable<EntitySubTypePicture> Pictures = new List<EntitySubTypePicture>();
}