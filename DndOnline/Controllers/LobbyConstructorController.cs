using DndOnline.Models;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndOnline.Controllers;

public class LobbyConstructorController : Controller
{
    private readonly ILobbyService _lobbyService;

    public LobbyConstructorController(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }
    
    public IActionResult Index()
    {
        var userName = User.Identity.Name;
        var model = new LobbyFormViewModel { Name = userName + "`s game" };
        return View(model);
    }
    
    [HttpGet]
    public PartialViewResult NewLobby()
    {
        var userName = User.Identity.Name;
        var model = new LobbyFormViewModel { Name = userName + "`s game" };
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

        return PartialView("Partial/NewLobby", model);
    }

    public PartialViewResult NewEnemy()
    {
        var model = new EnemyViewModel();
        return PartialView("Partial/NewEnemies", model);
    }
    
    [HttpPost]
    public IActionResult NewEnemy(EnemyViewModel model)
    {
        throw new NotImplementedException();
    }

    public PartialViewResult NewCharacter()
    {
        var model = new CharacterViewModel();
        return PartialView("Partial/NewCharacter", model);
    }
    
    [HttpPost]
    public IActionResult NewCharacter(CharacterViewModel model)
    {
        throw new NotImplementedException();
    }
    
    public IActionResult NewGameItem()
    {
        var model = new GameItemViewModel();
        return PartialView("Partial/NewGameItem", model);
    }
    
    [HttpPost]
    public IActionResult NewGameItem(GameItemViewModel model)
    {
        throw new NotImplementedException();
    }
}