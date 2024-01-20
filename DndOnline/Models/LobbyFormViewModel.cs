using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DndOnline.DataAccess.Objects;

namespace DndOnline.Models;

public class LobbyFormViewModel
{
    public Guid Id { get; set; }
    
    [DisplayName("Название лобби")]
    [Required(ErrorMessage = "Поле '{0}' обязательно для заполнения")]
    [MaxLength(50, ErrorMessage = "Недопустимое количество символов")] 
    public string Name { get; set; }

    [DisplayName("Описание")]
    public string? Description { get; set; }

    [Required]
    [DisplayName("Кол-во игроков")]
    public int MaxPlayers { get; set; } = 6;

    public int PLayersCount { get; set; } = 0;

    [Required]
    [DisplayName("Игровой мастер")]
    public string Master { get; set; }
}