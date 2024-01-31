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
        if (!ModelState.IsValid)
        {
            try
            {
                var lobby = _lobbyService.CreateLobby(model);
                return RedirectToAction("Index", new { id = lobby.Id });
            }
            catch (DbUpdateException ex)
            {
                var baseEx = ex.GetBaseException() as Npgsql.PostgresException;
                if (baseEx.SqlState == "23505") ViewBag.Alert = "Лобби с таким названием уже существует";
            }
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

        var sessionVal = HttpContext.Session.GetString("LobbyCreatures");
        if (!string.IsNullOrEmpty(sessionVal))
        {
            // model = JsonSerializer.Deserialize<List<CreatureViewModel>>(sessionVal);
        }

        return PartialView("Partial/NewCreatures", model);
    }

    [HttpPost]
    public ResponseModel NewCreature(CreatureViewModel model)
    {
        var response = new ResponseModel();

        if (!string.IsNullOrEmpty(model.Name))
        {
            
        }

        response.SetSuccess(model);

        return response;
    }

    public PartialViewResult NewCharacter()
    {
        var model = new List<CharacterViewModel>();

        var sessionVal = HttpContext.Session.GetString("LobbyCharacters");
        if (!string.IsNullOrEmpty(sessionVal))
        {
            model = JsonSerializer.Deserialize<List<CharacterViewModel>>(sessionVal);
        }

        return PartialView("Partial/NewCharacter", model);
    }

    [HttpPost]
    public ResponseModel NewCharacter(CharacterViewModel model)
    {
        var response = new ResponseModel();

        if (!string.IsNullOrEmpty(model.Name))
        {
            var enemiesList = new List<CharacterViewModel>();
            var sessionVal = HttpContext.Session.GetString("LobbyCharacters");

            if (!string.IsNullOrEmpty(sessionVal))
            {
                enemiesList = JsonSerializer.Deserialize<List<CharacterViewModel>>(sessionVal);
            }

            enemiesList.Add(model);

            HttpContext.Session.SetString("LobbyCharacters", JsonSerializer.Serialize(enemiesList));
            response.SetSuccess(model);
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
                FilePath = s.RelativePath
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