using System.Security.Claims;
using AuthService.Services.Interfaces;

namespace DndOnline.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
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
            context.Request.Headers.Add("Authorization", $"Bearer {jwt}");
        }
        else
        {
            var userName = jsonToken.Claims.FirstOrDefault(f => f.Type == "unique_name")?.Value;
            var refreshToken = userService.Get(userName).RefreshToken;
            if (!refreshToken.IsExpired)
            {
                tokenService.RefreshJwt(jwt);
                
                context.Session.SetString("JwtToken", jwt);
                context.Request.Headers.Add("Authorization", $"Bearer {jwt}");
            }
        }

        await _next(context);
    }
}