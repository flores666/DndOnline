using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DndOnline.Models;

public class EnemyViewModel
{
    [Required(ErrorMessage = "Это поле обязательно")]
    [DisplayName("Имя персонажа")]
    public string Name { get; set; }
    
    [DisplayName("Описание")]
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public FormFile? File { get; set; } 
}