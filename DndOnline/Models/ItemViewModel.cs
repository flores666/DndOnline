using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DndOnline.Models;

public class ItemViewModel
{
    
    [DisplayName("Название предмета")]
    [Required(ErrorMessage = "Поле '{0}' обязательно для заполнения")]
    [MaxLength(50, ErrorMessage = "Недопустимое количество символов")]
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string Description { get; set; }
    public FormFile? File { get; set; }
}