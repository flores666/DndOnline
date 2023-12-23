using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class LoginModel
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    [DisplayName("Имя")]
    public string Name { get; set; }
    
    [Required]
    [DisplayName("Пароль")]
    public string Password { get; set; }
    
    [Required]
    [DisplayName("Запомнить меня")]
    public bool RememberMe { get; set; }
}