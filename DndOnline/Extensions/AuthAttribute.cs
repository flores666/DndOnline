using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DndOnline.Extensions;

public class AuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.Cookies.TryGetValue("jwt", out string jwt);
        var result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn" }));
        if (string.IsNullOrEmpty(jwt))
        {
            context.Result = result;
        }
        else
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated) context.Result = result;
            else
            {
                var token = JsonSerializer.Deserialize<TokenModel>(jwt); 
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(token?.JWT) is not JwtSecurityToken) context.Result = result;
            }
        }
    }
}