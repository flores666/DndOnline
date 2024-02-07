using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class EntityTypePicture
{
    [Key]
    [ForeignKey("EntityType")]
    public Guid EntityTypeId { get; set; }
    public EntityType EntityType { get; set; }
    public string Path { get; set; }
}