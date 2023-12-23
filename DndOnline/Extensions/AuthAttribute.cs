using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DndOnline.Extensions;

public class AuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var jwt = context.HttpContext.Session.GetString("jwt");
        var result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn" }));
        if (string.IsNullOrEmpty(jwt))
        {
            context.Result = result;

        }
        else
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(jwt) is not JwtSecurityToken token) context.Result = result;
        }
    }
}