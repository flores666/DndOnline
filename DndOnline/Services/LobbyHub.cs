using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DndOnline.Services;

public class LobbyHub : Hub, ILobbyHubContext
{
    public IHubClients Clients { get; }
    
    public void JoinLobby(string playerName)
    {
        if (Clients != null) 
        Clients.All.SendAsync("JoinLobby", playerName);
    }

    public void LeaveLobby(string playerName)
    {
        if (Clients != null) 
        Clients.All.SendAsync("LeaveLobby", playerName);
    }

}