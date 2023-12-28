using DndOnline.Extensions;
using DndOnline.Models;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DndOnline.Controllers;

[Auth]
public class LobbyController : Controller
{
    private readonly ILobbyService _lobbyService;

    public LobbyController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }
    
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.Title = "Лобби";
    }

    public IActionResult Index(Guid id)
    {
        var lobby = _lobbyService.GetLobby(id);
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