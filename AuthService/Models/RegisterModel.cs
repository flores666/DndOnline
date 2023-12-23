using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class RegisterModel
{
    [Required]
    public Guid id { get; set; }
    
    [Required(ErrorMessage = "Поле не может быть пустым")]
    [DisplayName("Имя")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Поле не может быть пустым")]
    [DisplayName("Пароль")]
    [MinLength(5, ErrorMessage = "Пароль должен содержать минимум 5 символов")]
    [MaxLength(15, ErrorMessage = "Пароль должен содержать максимум 15 символов")]
    [RegularExpression(@"^.*(?=.*[a-zA-Z])(?=.*\d)(?=.*[!#$%&?]).*$", ErrorMessage = "Пароль должен содержать хотя бы одну цифру и символ (!,#,$,%,&,?)")]
    public string Password { get; set; }
    
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [Required(ErrorMessage = "Поле не может быть пустым")]
    [DisplayName("Повторите пароль")]
    public string RepeatedPassword { get; set; }
}