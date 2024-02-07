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

    public PartialViewResult NewCreature()
    {
        var model = new List<CreatureViewModel>();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        var draft = _lobbyService.GetCreatures(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new CreatureViewModel()
            {
                Name = s.Name,
                Description = s.Description,
                // FilePath = s.RelativePath
            }).ToList();
        }

        return PartialView("Partial/NewCreatures", model);
    }

    [HttpPost]
    public async Task<ResponseModel> NewCreature(CreatureViewModel model)
    {
        var response = new ResponseModel();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        if (!string.IsNullOrEmpty(model.Name))
        {
            response = await _lobbyService.AddCreatureAsync(new Guid(lobbyId), model);
        }

        return response;
    }

    public PartialViewResult NewCharacter()
    {
        var model = new List<CharacterViewModel>();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        var draft = _lobbyService.GetCharacters(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new CharacterViewModel()
            {
                Name = s.Name,
                Description = s.Description,
                // FilePath = s.RelativePath
            }).ToList();
        }

        return PartialView("Partial/NewCharacter", model);
    }

    [HttpPost]
    public async Task<ResponseModel> NewCharacter(CharacterViewModel model)
    {
        var response = new ResponseModel();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        if (!string.IsNullOrEmpty(model.Name))
        {
            response = await _lobbyService.AddCharacterAsync(new Guid(lobbyId), model);
        }

        return response;
    }

    public IActionResult NewItem()
    {
        var model = new List<ItemViewModel>();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        var draft = _lobbyService.GetItems(new Guid(lobbyId));

        if (draft != null)
        {
            model = draft.Select(s => new ItemViewModel()
            {
                Name = s.Name,
                Description = s.Description,
                // FilePath = s.RelativePath
            }).ToList();
        }

        return PartialView("Partial/NewItem", model);
    }

    [HttpPost]
    public async Task<ResponseModel> NewItem(ItemViewModel model)
    {
        var response = new ResponseModel();
        var lobbyId = HttpContext.Session.GetString("DraftLobbyId");

        if (!string.IsNullOrEmpty(model.Name))
        {
            response = await _lobbyService.AddItemAsync(new Guid(lobbyId), model);
        }

        return response;
    }

    public PartialViewResult CreaturePartialForm()
    {
        return PartialView("Partial/CreaturePartialView");
    }

    public PartialViewResult CharacterPartialForm()
    {
        return PartialView("Partial/CharacterPartialView");
    }

    public PartialViewResult ItemPartialForm()
    {
        return PartialView("Partial/ItemPartialView");
    }
}