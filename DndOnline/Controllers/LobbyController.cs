using DndOnline.Extensions;
using DndOnline.Models;
using DndOnline.Services;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
        
        if (lobby == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var playerId = HttpContext.User.Claims
            .FirstOrDefault(f => f.Type == "id")?.Value;

        var result = _lobbyService.ConnectUser(new Guid(playerId), lobby);
        if (!result.IsSuccess) RedirectToAction("Index", "Home");

        var curUserName = HttpContext.User.Identity.Name;
        HttpContext.Session.SetString("lobbyId", id.ToString());

        return lobby.Master == curUserName ? View("LobbyMaster", lobby) : View(lobby);
    }
}