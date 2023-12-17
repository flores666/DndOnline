using AuthService.DataAccess;
using AuthService.DataAccess.Objects;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public class UserService : IUserService
{
    private readonly AuthServiceDbContext _db;
    private readonly ITokenService _tokenService;

    public UserService(AuthServiceDbContext context, ITokenService tokenService)
    {
        _db = context;
        _tokenService = tokenService;
    }

    public Response Register(RegisterModel model)
    {
        if (Get(model.Name) != null) return new Response(StatusCodes.Status409Conflict, "Такой пользователь уже существует");
        var passwordHash = PasswordHasher.Hash(model.Password);
        var user = new User { Name = model.Name, PasswordHash = passwordHash };
        _db.Users.Add(user);
        _db.SaveChanges();
        return new Response(StatusCodes.Status201Created, "Пользователь успешно зарегистрировался");
    }
    
    public User Get(string name)
    {
        return _db.Users.FirstOrDefault(u => u.Name == name);
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

        var jwtToken = _tokenService.GenerateJwt(model.Name);
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
        
        return new Response(StatusCodes.Status200OK, tokenModel);
    }

    public Response Logout(string name)
    {
        var user = _db.Users.Include(u => u.RefreshToken).FirstOrDefault(u => u.Name == name);
        if (user == null) return new Response(StatusCodes.Status404NotFound, "Пользователь не найден");
        if (user.RefreshToken == null) return new Response(StatusCodes.Status400BadRequest, "Некорректный запрос");
        //var jwt = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //_tokenService.AddJwtToBlackList(jwt);
        _db.RefreshTokens.Remove(user.RefreshToken);
        _db.SaveChanges();

        return new Response(StatusCodes.Status200OK, "Пользователь успешно деавторизовался");
    }
}