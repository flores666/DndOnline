using System.Text.Json;
using DndOnline.DataAccess.Objects;
using DndOnline.Extensions;
using DndOnline.Models;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DndOnline.Controllers;

[Auth]
public class LobbyConstructorController : Controller
{
    private readonly ILobbyService _lobbyService;

    public LobbyConstructorController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionVal = HttpContext.Session.GetString("DraftLobbyName");
        ViewBag.Title = !string.IsNullOrEmpty(sessionVal) ? sessionVal : "Конструтор лобби";
    }

    public IActionResult Index()
    {
        var userName = User.Identity.Name;
        var userId = User.Claims.FirstOrDefault(f => f.Type == "id").Value;

        var model = new LobbyFormViewModel
        {
            Id = new Guid(),
            Master = userName,
            Name = userName + "`s game"
        };

        var draftLobby = _lobbyService.GetLobby(new Guid(userId), LobbyStatusType.Draft);
        if (draftLobby != null)
        {
            model = new LobbyFormViewModel
            {
                Master = User.Identity.Name,
                Id = draftLobby.Id,
                Name = draftLobby.Name,
                MaxPlayers = draftLobby.MaxPlayers,
                Description = draftLobby.Description
            };
        }
        else
        {
            _lobbyService.CreateLobby(model);
        }

        HttpContext.Session.SetString("DraftLobbyName", model.Name);
        HttpContext.Session.SetString("DraftLobbyId", model.Id.ToString());
        return View(model);
    }

    [HttpGet]
    public PartialViewResult NewLobby()
    {
        var model = new LobbyFormViewModel();
        var userId = User.Claims.FirstOrDefault(f => f.Type == "id").Value;
        var draftLobby = _lobbyService.GetLobby(new Guid(userId), LobbyStatusType.Draft);

        model = new LobbyFormViewModel
        {
            Master = User.Identity.Name,
            Id = draftLobby.Id,
            Name = draftLobby.Name,
            MaxPlayers = draftLobby.MaxPlayers,
            Description = draftLobby.Description
        };

        return PartialView("Partial/NewLobby", model);
    }

    [HttpPost]
    public IActionResult NewLobby(LobbyFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = _lobbyService.CreateLobby(model);
            if (!response.IsSuccess)
            {
                ViewBag.Alert = response.Message;
                return View("Index", model);
            }

            var lobby = response.Data as Lobby;
            return Redirect($"/lobby/{lobby.Id}");
        }
        else
        {
            ViewBag.Alert = "Ошибка создания лобби";
        }

        return PartialView("Index", model);
    }

    [HttpPost]
    public async Task<ResponseModel> NewEntity(EntityViewModel model)
    {
        var response = new ResponseModel();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        if (!string.IsNullOrEmpty(model.Name))
        {
            response = await _lobbyService.AddEntityAsync(new Guid(lobbyId), model);
        }

        return response;
    }

    public IActionResult NewEntity()
    {
        var model = new List<EntityViewModel>();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        var draft = _lobbyService.GetEntities(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new EntityViewModel()
            {
                Name = s.Name,
                Description = s.Description,
                // FilePath = s.RelativePath
            }).ToList();
        }

        return PartialView("Partial/NewEntity", model);
    }

    public PartialViewResult NewMap()
    {
        return PartialView("Partial/NewMap");
    }

    public PartialViewResult EntityPartialForm()
    {
        return PartialView("Partial/EntityPartialView");
    }
}