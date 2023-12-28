using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DndOnline.Services;

public class LobbyHub : Hub
{
    public async Task JoinLobby(string playerName)
    {
        if (Clients != null) 
        await Clients.All.SendAsync("JoinLobby", playerName);
    }

    public void LeaveLobby(string playerName)
    {
        if (Clients != null) 
        Clients.All.SendAsync("LeaveLobby", playerName);
    }

}