using System.Text.Json.Serialization;

namespace AuthService.Models;

public class TokenModel
{
    public string JWT { get; set; }
    
    public string RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpTime  { get; set; }
}