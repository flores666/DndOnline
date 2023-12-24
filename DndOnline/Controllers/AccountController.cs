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
    public async Task<IActionResult> SignIn(LoginModel userLogin)
    {
        if (!ModelState.IsValid) return View();
        var response = _userService.Authenticate(userLogin);
        if (response.IsSuccess) return Redirect("/home/index");
        
        ViewData["Alert"] = response.Message;
        return View();

    }

    /// <summary>
    /// выход из аккаунта
    /// </summary>
    [Auth]
    [HttpPost("/sign-out")]
    public IActionResult SignOut()
    {
        _userService.Logout(HttpContext.User.Identity.Name);
        return Redirect(Url.Action("SignIn"));
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
        if (response.StatusCode != StatusCodes.Status201Created) return View();

        var loginModel = new LoginModel() {Name = registerModel.Name, Password = registerModel.Password};
        _userService.Authenticate(loginModel);

        return Redirect(Url.Action("Index", "Home"));
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
}