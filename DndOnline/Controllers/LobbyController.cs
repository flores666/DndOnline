using DndOnline.DataAccess.Objects;
using DndOnline.Extensions;
using DndOnline.Models;
using DndOnline.Services;
using DndOnline.Services.Interfaces;
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
        
        var playerId = HttpContext.User.Claims
            .FirstOrDefault(f => f.Type == "id")?.Value;

        var result = _lobbyService.ConnectUser(new Guid(playerId), lobby);
        if (!result.IsSuccess) RedirectToAction("Index", "Home");

        HttpContext.Session.SetString("lobbyId", id.ToString());
        return View(lobby);
    }

    [HttpPost]
    public IActionResult CreateLobby(LobbyFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lobby = _lobbyService.CreateLobby(model);
        }
        else
        {
            ViewBag.Alert = "Ошибка создания лобби";
        }

        return RedirectToAction("Index", "Home");
    }
}