using DndOnline.DataAccess;
using DndOnline.Extensions;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DndOnline.Controllers;

[Auth]
public class HomeController : Controller
{
    private ILobbyService _lobbyService;
    
    public HomeController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.Title = "Домашняя страница";
        ViewData["Name"] = HttpContext.User.Identity.Name;
    }

    public IActionResult Index()
    {
        var lobbies = _lobbyService.GetLobbies();
        return View(lobbies);
    }
}