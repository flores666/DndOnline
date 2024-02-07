using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DndOnline.DataAccess.Objects;

namespace DndOnline.Models;

public class CreatureViewModel
{
    [DisplayName("Имя существа")]
    [Required(ErrorMessage = "Поле '{0}' обязательно для заполнения")]
    [MaxLength(50, ErrorMessage = "Недопустимое количество символов")]
    public string Name { get; set; }
    
    [DisplayName("Описание")]
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public IFormFile? File { get; set; }
}