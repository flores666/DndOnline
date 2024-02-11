using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class Picture
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Path { get; set; }
    public IEnumerable<Entity> Entities { get; set; }
}