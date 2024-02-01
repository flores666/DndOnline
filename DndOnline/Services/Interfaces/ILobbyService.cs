using DndOnline.DataAccess.Objects;
using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface ILobbyService
{
    public Lobby CreateLobby(LobbyFormViewModel model);
    public ResponseModel DeleteLobby();
    public Lobby GetLobby(string name);
    public Lobby GetLobby(Guid id);
    public Lobby GetLobby(Guid userId, LobbyStatusType status);
    public List<Lobby> GetLobbies(int page = 1, int pageSize = 20);
    public List<Lobby> GetLobbies(string input, int page = 1, int pageSize = 20);
    public ResponseModel ConnectUser(string userName);
    public ResponseModel ConnectUser(Guid userId, Lobby lobby);
    public ResponseModel DisconnectUser(Guid userId, Guid lobbyId);
    public Task<ResponseModel> AddItemAsync(Guid lobbyId, ItemViewModel model);
    public Task<ResponseModel> AddCreatureAsync(Guid lobbyId, CreatureViewModel model);
    public Task<ResponseModel> AddCharacterAsync(Guid lobbyId, CharacterViewModel model);
    public Task<ResponseModel> AddMapAsync(Guid lobbyId, MapViewModel model);
    public List<Item> GetItems(Guid lobbyId);
    public List<Creature> GetCreatures(Guid lobbyId);
    public List<Character> GetCharacters(Guid lobbyId);
    public List<Map> GetMaps(Guid lobbyId);
}