using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DndOnline.Models;

public class EnemyViewModel
{
    [DisplayName("Имя персонажа")]
    [Required(ErrorMessage = "Поле '{0}' обязательно для заполнения")]
    [MaxLength(50, ErrorMessage = "Недопустимое количество символов")]
    public string Name { get; set; }
    
    [DisplayName("Описание")]
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public FormFile? File { get; set; } 
}