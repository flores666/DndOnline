using System.Security.Claims;
using DndOnline.Extensions;
using DndOnline.Models;
using DndOnline.Services;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;

namespace DndOnline.Controllers;

[Auth]
public class LobbyController : Controller
{
    private readonly ILobbyService _lobbyService;
    private readonly IHubContext<LobbyHub> _lobbyHubContext;

    public LobbyController(ILobbyService lobbyService, IHubContext<LobbyHub> lobbyHubContext)
    {
        _lobbyService = lobbyService;
        _lobbyHubContext = lobbyHubContext;
    }
    
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.Title = "Лобби";
    }

    public IActionResult Index(Guid id)
    {
        var lobby = _lobbyService.GetLobby(id);
        var playerName = HttpContext.User.Claims
            .FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value;
        _lobbyHubContext.Clients.All.SendAsync("JoinLobby", playerName);
        return View(lobby);
    }

    [HttpPost]
    public IActionResult CreateLobby(LobbyFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _lobbyService.CreateLobby(model);
        }
        else
        {
            ViewBag.Alert = "Ошибка создания лобби";
        }

        return RedirectToAction("Index", "Home");
    }
}