using DndOnline.DataAccess;
using DndOnline.Extensions;
using DndOnline.Models;
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
        ViewBag.Title = "Список лобби";
        ViewData["Name"] = HttpContext.User.Identity.Name;
    }

    public IActionResult Index()
    {
        var lobbies = _lobbyService.GetLobbies();
        var model = lobbies.Select(s => new LobbyFormViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Master = s.Master,
            MaxPlayers = s.MaxPlayers,
            PLayersCount = s.Players.Count
        });
        return View(model);
    }
    
    public PartialViewResult SearchLobby(string input)
    {
        IEnumerable<LobbyFormViewModel> model = new List<LobbyFormViewModel>();
        
        var lobbies = _lobbyService.GetLobbies(input);
        model = lobbies.Select(s => new LobbyFormViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Master = s.Master,
            MaxPlayers = s.MaxPlayers,
            PLayersCount = s.Players.Count
        });
        
        return PartialView("Partial/LobbyList", model);
    }
}