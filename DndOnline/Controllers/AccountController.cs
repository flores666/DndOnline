using AuthService.Models;
using AuthService.Services.Interfaces;
using DndOnline.Extensions;
using DndOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DndOnline.Controllers;

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

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Вход";
    }

    /// <summary>
    /// получение данных о текущем аккаунте
    /// </summary>
    [Auth]
    [HttpGet("me")]    
    public IActionResult Me()
    {
        var userViewModel = new UserViewModel
        {
            Name = "default"
        };

        return Ok(userViewModel);
    }

    [HttpGet("/login")]
    public IActionResult SignIn()
    {
        return View();
    }
    
    /// <summary>
    /// получение нового jwt токена пользователя
    /// </summary>  
    [HttpPost("/login")]
    public IActionResult SignIn(LoginModel userLogin)
    {
        if (!ModelState.IsValid) return View();
        var response = _userService.Authenticate(userLogin);
        if (response.StatusCode == StatusCodes.Status200OK) 
            HttpContext.Session.SetString("jwt", (response.Data as TokenModel).JWT);
        return Redirect("/home/index");
    }

    /// <summary>
    /// выход из аккаунта
    /// </summary>
    [Auth]
    [HttpPost("/sign-out")]
    public IActionResult SignOut()
    {
        var name = HttpContext.User.Identity.Name;
        if (name == null) return BadRequest();
        var response = _userService.Logout(name);
        if (response.StatusCode == StatusCodes.Status200OK) HttpContext.Response.Cookies.Delete("refreshToken");
        return StatusCode(response.StatusCode, response.Data);
    }

    [HttpGet("/sign-up")]
    public IActionResult SignUp()
    {
        return View();
    }
    
    /// <summary>
    /// регистрация нового аккаунта
    /// </summary>
    [HttpPost("/sign-up")]
    public IActionResult SignUp(RegisterModel registerModel)
    {
        if (!ModelState.IsValid) return View();
        var response = _userService.Register(registerModel);
        return Redirect("/login");
    }

    /// <summary>
    /// обновление своего аккаунта
    /// </summary>
    [Auth]
    [HttpPut]
    public IActionResult Update(UserViewModel user)
    {
        return BadRequest();
    }
    
    /// <summary>
    /// обновление устаревшего JWT
    /// </summary>
    [HttpPost]
    public IActionResult RefreshToken()
    {
        if (HttpContext.User.Claims.FirstOrDefault() != null) return StatusCode(StatusCodes.Status403Forbidden, "Токен еще не истек");
        var jwt = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (jwt == null) return Unauthorized("Пользователь не авторизован");
        var response = _tokenService.RefreshJwt(jwt);
        if (response.StatusCode == StatusCodes.Status200OK) SetRefreshTokenSession((response.Data as TokenModel).RefreshToken);
        return StatusCode(response.StatusCode, response.Data);
    }
    
    private void SetRefreshTokenSession(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["RefreshToken:LifetimeDays"]))
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}