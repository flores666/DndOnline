using System.Security.Claims;
using System.Text.Json;
using AuthService.Models;
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
        // Проверяем, есть ли токен в куках
        var containsValue = context.Request.Cookies.TryGetValue("jwt", out string jwt);

        // Если токен отсутствует, выполняем следующий обработчик в цепочке middleware
        if (!containsValue)
        {
            await _next(context);
            return;
        }
        
        TokenModel token;
        try
        {
            token = JsonSerializer.Deserialize<TokenModel>(jwt);
        }
        catch
        {
            await _next(context);
            return;
        }

        // Если токен присутствует, проверяем, истек ли он
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var accessToken = handler.ReadToken(token.JWT) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

        if (accessToken.ValidTo > DateTime.UtcNow)
        {
            context.Request.Headers.Authorization = new StringValues("Bearer " + token.JWT);
        }
        else
        {
            if (token.RefreshTokenExpTime > DateTime.Now)
            {
                var response = tokenService.RefreshToken(token);
                if (response.IsSuccess)
                {
                    var newToken = response.Data as TokenModel;
                    context.Response.Cookies.Append("jwt", JsonSerializer.Serialize(newToken));
                    context.Request.Headers.Authorization = new StringValues("Bearer " + newToken.JWT);
                }
            }
        }

        await _next(context);
    }
}