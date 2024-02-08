using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class EntitySubTypePicture
{
    [Key]
    [ForeignKey("SubType")]
    public Guid SubTypeId { get; set; }
    public EntitySubType SubType { get; set; }
    public string Path { get; set; }
}