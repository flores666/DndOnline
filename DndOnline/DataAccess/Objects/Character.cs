using System.ComponentModel.DataAnnotations;

namespace DndOnline.DataAccess.Objects;

public class Character
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required] 
    public string FullFilePath { get; set; }

    [Required]
    public byte[] File { get; set; }
}