using System.Security.Claims;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Primitives;

namespace DndOnline.Middlewares;

public class TokenInHeaderMiddleware
{
    private readonly RequestDelegate _next;
    
    public TokenInHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenService tokenService, IUserService userService)
    {
        // Проверяем, есть ли токен в сессии
        string jwt = context.Session.GetString("jwt");

        // Если токен отсутствует, выполняем следующий обработчик в цепочке middleware
        if (string.IsNullOrEmpty(jwt))
        {
            await _next(context);
            return;
        }

        // Если токен присутствует, проверяем, истек ли он
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

        if (jsonToken.ValidTo > DateTime.UtcNow)
        {
            context.Request.Headers.Authorization = new StringValues("Bearer " + jwt);
        }
        else
        {
            var userName = jsonToken.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value;
            var refreshToken = userService.Get(userName)?.RefreshToken;
            if (!refreshToken.IsExpired)
            {
                tokenService.RefreshTokens(jwt);
                
                context.Session.SetString("jwt", jwt);
                context.Request.Headers.Authorization = new StringValues("Bearer " + jwt);
            }
        }

        await _next(context);
    }
}