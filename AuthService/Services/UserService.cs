using System.Security.Claims;
using System.Text.Json;
using AuthService.DataAccess;
using AuthService.DataAccess.Objects;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace AuthService.Services;

public class UserService : IUserService
{
    private readonly AuthServiceDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly HttpContext _httpContext;
    
    public UserService(AuthServiceDbContext context, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _db = context;
        _tokenService = tokenService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public Response Register(RegisterModel model)
    {
        var response = new Response(StatusCodes.Status409Conflict, "Такой пользователь уже существует");
        
        if (_db.Users.FirstOrDefault(f => f.Name == model.Name) == null)
        {
            var passwordHash = PasswordHasher.Hash(model.Password);
            var user = new User {Name = model.Name, PasswordHash = passwordHash};

            _db.Users.Add(user);
            var result = _db.SaveChanges();

            if (result > 0)
            {
                response.StatusCode = StatusCodes.Status201Created;
                response.Message = "Успешно!";
            }
        }
        return response;
    }

    public User Get(Guid id)
    {
        return _db.Users.Include(i => i.RefreshToken).FirstOrDefault(u => u.Id == id);
    }

    public User Get(string Name)
    {
        return _db.Users.Include(i => i.RefreshToken).FirstOrDefault(u => u.Name == Name);
    }

    public void Update(User user)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Response Authenticate(LoginModel model)
    {
        var user = _db.Users.Include(u => u.RefreshToken).FirstOrDefault(u => u.Name == model.Name);
        if (user == null) return new Response(StatusCodes.Status404NotFound, "Пользователь не найден");
        if (!PasswordHasher.Validate(model.Password, user.PasswordHash))
            return new Response(StatusCodes.Status400BadRequest, "Данные не верны или отсутствуют");

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim("unique_name", user.Name));
        claims.Add(new Claim("id", user.Id.ToString()));
        
        var jwtToken = _tokenService.GenerateJwt(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        if (!string.IsNullOrEmpty(user.RefreshToken?.Token)) _db.RefreshTokens.Remove(user.RefreshToken);

        user.RefreshToken = refreshToken;
        _db.Update(user);
        _db.SaveChanges();

        var tokenModel = new TokenModel
        {
            JWT = jwtToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpTime = refreshToken.ExpiryTime
        };

        if (_httpContext != null)
        {
            _httpContext.Response.Cookies.Append("jwt", JsonSerializer.Serialize(tokenModel));
            _httpContext.Request.Headers.Authorization = new StringValues("Bearer " + tokenModel.JWT);
        }

        return new Response(StatusCodes.Status200OK, "Успешно", tokenModel);
    }

    public Response Logout(string name)
    {
        var user = _db.Users.Include(u => u.RefreshToken).FirstOrDefault(u => u.Name == name);
        if (user == null) return new Response(StatusCodes.Status404NotFound, "Пользователь не найден");
        if (user.RefreshToken == null) return new Response(StatusCodes.Status400BadRequest, "Некорректный запрос");

        _db.RefreshTokens.Remove(user.RefreshToken);
        var result = _db.SaveChanges();
        
        if (result > 0) _httpContext.Response.Cookies.Delete("jwt");
        
        return new Response(StatusCodes.Status200OK, "Пользователь успешно деавторизовался");
    }
    
    public Response Logout(Guid id)
    {
        var user = _db.Users.Include(u => u.RefreshToken).FirstOrDefault(u => u.Id == id);
        if (user == null) return new Response(StatusCodes.Status404NotFound, "Пользователь не найден");
        if (user.RefreshToken == null) return new Response(StatusCodes.Status400BadRequest, "Некорректный запрос");

        _db.RefreshTokens.Remove(user.RefreshToken);
        var result = _db.SaveChanges();
        
        if (result > 0) _httpContext.Response.Cookies.Delete("jwt");
        
        return new Response(StatusCodes.Status200OK, "Пользователь успешно деавторизовался");
    }
}