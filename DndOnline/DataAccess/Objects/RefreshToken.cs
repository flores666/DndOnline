using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class RefreshToken
{
    [Key]
    [Required]
    [Column(TypeName = "text")]
    public string Token { get; set; }
    
    [Required]
    public DateTime ExpiryTime { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiryTime;
}