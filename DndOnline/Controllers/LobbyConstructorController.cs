using System.Text.Json;
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
        var sessionVal = HttpContext.Session.GetString("LobbyFormViewModel");
        var model = new LobbyFormViewModel();

        if (!string.IsNullOrEmpty(sessionVal))
        {
            model = JsonSerializer.Deserialize<LobbyFormViewModel>(sessionVal);
            ViewBag.Title = model.Name;
        }
        else
        {
            ViewBag.Title = "Конструтор лобби";
        }
    }

    public IActionResult Index()
    {
        var userName = User.Identity.Name;
        var model = new LobbyFormViewModel {Name = userName + "`s game"};
        HttpContext.Session.SetString("LobbyFormViewModel", JsonSerializer.Serialize(model));
        return View(model);
    }

    [HttpGet]
    public PartialViewResult NewLobby()
    {
        var sessionVal = HttpContext.Session.GetString("LobbyFormViewModel");
        var model = new LobbyFormViewModel();

        if (string.IsNullOrEmpty(sessionVal))
        {
            var userName = User.Identity.Name;
            model.Name = userName + "`s game";
            HttpContext.Session.SetString("LobbyFormViewModel", JsonSerializer.Serialize(model));
        }
        else model = JsonSerializer.Deserialize<LobbyFormViewModel>(sessionVal);

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
                return RedirectToAction("Index", new {id = lobby.Id});
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

        return PartialView("Partial/NewLobby", model);
    }

    public PartialViewResult NewEnemy()
    {
        var model = new List<EnemyViewModel>();

        var sessionVal = HttpContext.Session.GetString("LobbyEnemies");
        if (!string.IsNullOrEmpty(sessionVal))
        {
            model = JsonSerializer.Deserialize<List<EnemyViewModel>>(sessionVal);
        }

        return PartialView("Partial/NewEnemies", model);
    }

    [HttpPost]
    public ResponseModel NewEnemy(EnemyViewModel model)
    {
        var response = new ResponseModel();

        if (!string.IsNullOrEmpty(model.Name))
        {
            var enemiesList = new List<EnemyViewModel>();
            var sessionVal = HttpContext.Session.GetString("LobbyEnemies");

            if (!string.IsNullOrEmpty(sessionVal))
            {
                enemiesList = JsonSerializer.Deserialize<List<EnemyViewModel>>(sessionVal);
            }

            enemiesList.Add(model);

            HttpContext.Session.SetString("LobbyEnemies", JsonSerializer.Serialize(enemiesList));
            response.SetSuccess(model);
        }

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

        var sessionVal = HttpContext.Session.GetString("LobbyItems");
        if (!string.IsNullOrEmpty(sessionVal))
        {
            model = JsonSerializer.Deserialize<List<ItemViewModel>>(sessionVal);
        }

        return PartialView("Partial/NewItem", model);
    }

    [HttpPost]
    public ResponseModel NewItem(ItemViewModel model)
    {
        var response = new ResponseModel();

        if (!string.IsNullOrEmpty(model.Name))
        {
            var enemiesList = new List<ItemViewModel>();
            var sessionVal = HttpContext.Session.GetString("LobbyItems");

            if (!string.IsNullOrEmpty(sessionVal))
            {
                enemiesList = JsonSerializer.Deserialize<List<ItemViewModel>>(sessionVal);
            }

            enemiesList.Add(model);

            HttpContext.Session.SetString("LobbyItems", JsonSerializer.Serialize(enemiesList));
            response.SetSuccess(model);
        }

        return response;
    }
}