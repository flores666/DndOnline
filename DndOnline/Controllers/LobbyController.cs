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

    [HttpGet("/lobby/{id?}")]
    public IActionResult Index(Guid id)
    {
        var lobby = _lobbyService.GetLobbyFull(id);

        if (lobby == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // lobby.Scenes = lobby.Scenes.OrderBy(o => o.Sort).ToList();
        var sortResult = SortScenesAndFindLastPlayed(lobby.Scenes);

        lobby.Scenes = sortResult.Item1;
        ViewBag.LastPlayed = sortResult.Item2;
        
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = _lobbyService.ConnectUser(userId, lobby);
        if (!result.IsSuccess) RedirectToAction("Index", "Home");

        Response.Cookies.Append("cur_lobby", id.ToString());
        Response.Cookies.Append("cur_lobby_master", lobby.MasterId.ToString());
        ViewBag.Title = lobby.Name;
        var isMaster = lobby.MasterId == userId;

        if (!isMaster) return View(lobby);

        ViewBag.Tokens = _lobbyService.GetEntities(userId);
        ViewBag.Maps = _lobbyService.GetMaps(userId);
        return View("Master", lobby);
    }

    [HttpPost]
    public async Task<ResponseModel> SaveScene(string json, Guid sceneId)
    {
        Request.Cookies.TryGetValue("cur_lobby", out var lobbyId);

        ResponseModel response = await _lobbyService.SaveSceneAsync(sceneId, json, Guid.Parse(lobbyId));
        return response;
    }

    [HttpPost]
    public async Task<ResponseModel> CreateScene(string name)
    {
        Request.Cookies.TryGetValue("cur_lobby", out var lobbyId);

        ResponseModel response = await _lobbyService.SaveSceneAsync(Guid.Empty, "", Guid.Parse(lobbyId), name);
        return response;
    }

    public async Task<ResponseModel> GetScene(Guid id)
    {
        var response = new ResponseModel();
        var scene = await _lobbyService.GetSceneAsync(id);
        if (scene == null) return response;
        
        response.SetSuccess(scene);
        
        Request.Cookies.TryGetValue("cur_lobby_master", out var masterId);
        if (User.Claims.FirstOrDefault(w => w.Type == "id").Value == masterId)
        {
            scene.LastPlayed = DateTime.Now.ToUniversalTime();
            _lobbyService.SaveSceneAsync(scene);
        }

        return response;
    }

    /// <summary>
    /// Сортирует сцены по полю sort и возвращает id последней игр
    /// </summary>
    /// <param name="scenes"></param>
    /// <returns></returns>
    private Tuple<List<Scene>, Guid> SortScenesAndFindLastPlayed(IEnumerable<Scene> scenes)
    {
        var result = scenes.Aggregate(
            (sortedList: new List<Scene>(), maxDateItem: (Scene) null),
            (accumulator, currentItem) =>
            {
                // Вставляем текущий элемент в отсортированную позицию по полю sort
                int indexToInsert = accumulator.sortedList.BinarySearch(currentItem, Comparer<Scene>.Create((x, y) => x.Sort.CompareTo(y.Sort)));
                if (indexToInsert < 0)
                    indexToInsert = ~indexToInsert;
                accumulator.sortedList.Insert(indexToInsert, currentItem);
                
                if (accumulator.maxDateItem == null || currentItem.LastPlayed > accumulator.maxDateItem.LastPlayed)
                {
                    accumulator.maxDateItem = currentItem;
                }

                return accumulator;
            });

        return new Tuple<List<Scene>, Guid>(result.sortedList, result.maxDateItem?.Id ?? Guid.Empty);
    }
}