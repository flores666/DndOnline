using System.Security.Claims;
using AuthService.Models;
using AuthService.Services.Interfaces;
using DndOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DndOnline.Controllers;

[Produces("application/json")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AccountController(ILogger<AccountController> logger,
        IUserService userService, ITokenService tokenService,
        IConfiguration configuration)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    /// <summary>
    /// получение данных о текущем аккаунте
    /// </summary>
    /// <response code="200">Получение данных о текущем аккаунте.</response>
    /// <response code="401">Пользователь не авторизорван</response>
    /// <response code="404">Пользователь не найден</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpGet]
    [Authorize]
    [Route("/Me")]
    [ProducesResponseType(typeof(UserViewModel), 200)]
    public IActionResult Me()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
            return Unauthorized("Пользователь не авторизован");
        var claims = identity.Claims;
        var name = identity.Name;

        var userViewModel = new UserViewModel
        {
            Name = name
        };

        return Ok(userViewModel);
    }

    /// <summary>
    /// получение нового jwt токена пользователя
    /// </summary>  
    /// <param name="body"></param>
    /// <response code="200">Пользователь успешно авторизовался</response>
    /// <response code="401">Данные не верны или отсутствуют</response>
    /// <response code="404">Пользователь не найден</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpPost]
    [Route("/SignIn")]
    [ProducesResponseType(typeof(TokenModel), 200)]
    public IActionResult SignIn(LoginModel userLogin)
    {
        if (!ModelState.IsValid) return StatusCode(StatusCodes.Status400BadRequest, "Некорректный запрос");
        var response = _userService.Authenticate(userLogin);
        if (response.StatusCode == StatusCodes.Status200OK) SetRefreshTokenCookie((response.Data as TokenModel).RefreshToken);
        return StatusCode(response.StatusCode, response.Data);
    }

    /// <summary>
    /// выход из аккаунта
    /// </summary>
    /// <response code="200">Пользователь успешно вышел из аккаунта</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpPost, Authorize]
    [Route("/SignOut")]
    public IActionResult SignOut()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
            return Unauthorized("Пользователь не авторизован");
        var name = identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Name).Value;
        if (name == null) return BadRequest();
        var response = _userService.Logout(name);
        if (response.StatusCode == StatusCodes.Status200OK) HttpContext.Response.Cookies.Delete("refreshToken");
        return StatusCode(response.StatusCode, response.Data);
    }

    /// <summary>
    /// регистрация нового аккаунта
    /// </summary>
    /// <param name="body"></param>
    /// <response code="201">Пользователь успешно зарегистрировался</response>
    /// <response code="409">Такой пользователь уже существует </response>
    /// <response code="400">Введенные данные некорректны</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpPost]
    [Route("/SignUp")]
    public IActionResult SignUp(RegisterModel registerModel)
    {
        if (!ModelState.IsValid) return StatusCode(StatusCodes.Status400BadRequest, "Введенные данные некорректны");
        var response = _userService.Register(registerModel);
        return StatusCode(response.StatusCode, response.Data);
    }

    /// <summary>
    /// обновление своего аккаунта
    /// </summary>
    /// <param name="body"></param>
    /// <response code="200">Данные успешно обновлены</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpPut, Authorize]
    [Route("/Update")]
    public IActionResult Update(UserViewModel user)
    {
        return BadRequest();
    }
    
    /// <summary>
    /// обновление устаревшего JWT
    /// </summary>
    /// <param name="body"></param>
    /// <response code="200">Токен успешно обновлен</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Токен еще не истек</response>
    /// <response code="500">Непредвиденная ошибка</response>
    [HttpPost]
    [Route("/RefreshToken")]
    [ProducesResponseType(typeof(TokenModel), 200)]
    public IActionResult RefreshToken()
    {
        if (HttpContext.User.Claims.FirstOrDefault() != null) return StatusCode(StatusCodes.Status403Forbidden, "Токен еще не истек");
        var jwt = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (jwt == null) return Unauthorized("Пользователь не авторизован");
        var response = _tokenService.RefreshJwt(jwt);
        if (response.StatusCode == StatusCodes.Status200OK) SetRefreshTokenCookie((response.Data as TokenModel).RefreshToken);
        return StatusCode(response.StatusCode, response.Data);
    }
    
    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["RefreshToken:LifetimeDays"]))
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}