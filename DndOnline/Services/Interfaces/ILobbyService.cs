using DndOnline.DataAccess.Objects;
using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface ILobbyService
{
    public ResponseModel CreateLobby(LobbyFormViewModel model);
    public ResponseModel DeleteLobby();
    public Lobby GetLobby(string name);
    public Lobby GetLobby(Guid id);
    public Lobby GetLobby(Guid userId, LobbyStatusType status);
    public Lobby GetLobbyFull(Guid id);
    public List<Lobby> GetLobbies(int page = 1, int pageSize = 20, string input = null);
    public ResponseModel ConnectUser(string userName);
    public ResponseModel ConnectUser(Guid userId, Lobby lobby);
    public ResponseModel DisconnectUser(Guid userId, Guid lobbyId);
    public Task<ResponseModel> AddEntityAsync(Guid lobbyId, EntityViewModel model);
    public Task<ResponseModel> AddMapAsync(Guid lobbyId, MapViewModel model);
    public List<EntityViewModel> GetEntities(Guid userId);
    public List<SceneViewModel> GetScenes(Guid lobbyId);
    public Task<ResponseModel> SaveSceneAsync(Guid sceneId, string json, Guid lobbyId, string? name = null);
    public List<MapViewModel> GetMaps(Guid userId);
    public Task<Scene> GetSceneAsync(Guid id);
}