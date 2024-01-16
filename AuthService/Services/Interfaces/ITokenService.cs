using System.Security.Claims;
using AuthService.DataAccess.Objects;
using AuthService.Models;

namespace AuthService.Services.Interfaces;

public interface ITokenService
{
    public string GenerateJwt(IEnumerable<Claim> claims);
    public RefreshToken GenerateRefreshToken();
    public Response RefreshToken(TokenModel token);
    public string GetValueFromJwt(string jwt, string param);
}