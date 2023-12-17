using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class RegisterModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [MinLength(5, ErrorMessage = "Пароль должен содержать минимум 5 символов")]
    [MaxLength(15, ErrorMessage = "Пароль должен содержать максимум 15 символов")]
    [RegularExpression(@"^.*(?=.*[a-zA-Z])(?=.*\d)(?=.*[!#$%&?]).*$", ErrorMessage = "Пароль должен содержать хотя бы одну цифру")]
    public string Password { get; set; }
}