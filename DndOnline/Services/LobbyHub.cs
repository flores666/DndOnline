using DndOnline.DataAccess.Objects;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DndOnline.Services;

public class LobbyHub : Hub
{
    private readonly HttpContext _httpContext;
    private readonly ILobbyService _lobbyService;
    
    public LobbyHub(IHttpContextAccessor httpContextAccessor, ILobbyService lobbyService)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _lobbyService = lobbyService;
    }
    
    public async Task JoinLobby()
    {
        var playerName = _httpContext.User.Identity.Name;
        if (Clients != null) await Clients.All.SendAsync("JoinLobby", playerName);
    }

    public async Task LeaveLobby()
    {
        var playerId = _httpContext.User.Claims.FirstOrDefault(w => w.Type == "id").Value;
        var lobbyIdString = _httpContext.Session.GetString("lobbyId");
        var lobbyId = new Guid(lobbyIdString);
        
        var response = _lobbyService.DisconnectUser(new Guid(playerId), lobbyId);
        var disconnectedUser = response.Data as User;
        
        if (Clients != null) await Clients.All.SendAsync("LeaveLobby", disconnectedUser.Name);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await LeaveLobby();
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        await JoinLobby();
        await base.OnConnectedAsync();
    }
}