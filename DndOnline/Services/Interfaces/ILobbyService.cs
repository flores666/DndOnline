using DndOnline.DataAccess.Objects;
using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface ILobbyService
{
    public Lobby CreateLobby(LobbyFormViewModel model);
    public ResponseModel DeleteLobby();
    public Lobby GetLobby(string name);
    public Lobby GetLobby(Guid id);
    public List<Lobby> GetLobbies(int page = 1, int pageSize = 20);
    public List<Lobby> GetLobbies(string input, int page = 1, int pageSize = 20);
    public ResponseModel ConnectUser(string userName);
    public ResponseModel ConnectUser(Guid userId, Lobby lobby);
    public ResponseModel DisconnectUser(Guid userId, Guid lobbyId);
}