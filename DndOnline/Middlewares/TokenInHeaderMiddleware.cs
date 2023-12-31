﻿using System.Security.Claims;
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
            var userName = jsonToken.Claims.FirstOrDefault(f => f.Type == "unique_name")?.Value;
            var refreshToken = userService.Get(userName)?.RefreshToken;
            if (!refreshToken.IsExpired)
            {
                var response = tokenService.RefreshTokens(jwt);
                if (response.IsSuccess)
                {
                    var token = response.Data as TokenModel;
                    context.Session.SetString("jwt", token.JWT);
                    context.Request.Headers.Authorization = new StringValues("Bearer " + token.JWT);
                }
            }
        }

        await _next(context);
    }
}