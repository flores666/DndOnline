using Microsoft.AspNetCore.SignalR;

namespace DndOnline.Services.Interfaces;

public interface ILobbyHubContext : IHubContext<LobbyHub>
{
    public void JoinLobby(string playerName);
    
    public void LeaveLobby(string playerName);
}