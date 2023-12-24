using AuthService.DataAccess.Objects;
using AuthService.Models;

namespace AuthService.Services.Interfaces;

public interface ITokenService
{
    public string GenerateJwt(string userName);
    public RefreshToken GenerateRefreshToken();
    public Response RefreshTokens(string jwt);
    public string GetValueFromJwt(string jwt, string param);
}