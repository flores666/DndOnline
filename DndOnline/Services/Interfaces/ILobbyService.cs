using DndOnline.DataAccess.Objects;
using DndOnline.Models;

namespace DndOnline.Services.Interfaces;

public interface ILobbyService
{
    public Lobby CreateLobby(string name, int maxPlayers);
    public ResponseModel DeleteLobby();
    public Lobby GetLobby();
    public ResponseModel ConnectUser(string userName);
}