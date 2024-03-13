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
            var res = _lobbyService.CreateLobby(model);
            if (res.IsSuccess)
            {
                model = res.Data as LobbyFormViewModel;
            }
            else return Redirect("/");
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

            var lobby = response.Data as LobbyFormViewModel;
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
        var lobbyId = User.Claims.FirstOrDefault(w => w.Type == "id").Value;

        var draft = _lobbyService.GetEntities(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new EntityViewModel()
            {
                Name = s.Name,
                Description = s.Description,
                FilePath = s.FilePath
            }).ToList();
        }

        return PartialView("Partial/NewEntity", model);
    }

    public PartialViewResult NewMap()
    {
        var model = new List<MapViewModel>();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        var draft = _lobbyService.GetMaps(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new MapViewModel()
            {
                Name = s.Name,
                FilePath = s.FilePath
            }).ToList();
        }

        return PartialView("Partial/NewMap", model);
    }

    [HttpPost]
    public async Task<ResponseModel> NewMap(MapViewModel model)
    {
        var response = new ResponseModel();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        if (!string.IsNullOrEmpty(model.Name))
        {
            response = await _lobbyService.AddMapAsync(new Guid(lobbyId), model);
        }

        return response;
    }

    public PartialViewResult EntityPartialForm()
    {
        return PartialView("Partial/EntityPartialView");
    }

    public PartialViewResult MapPartialForm()
    {
        return PartialView("Partial/MapPartialView");
    }
}