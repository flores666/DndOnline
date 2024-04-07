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
        _httpContext.Request.Cookies.TryGetValue("cur_lobby", out var lobbyId);
        var userName = _httpContext.User.Identity.Name;
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        await Clients.Group(lobbyId).SendAsync("JoinLobby", userName);
    }

    public async Task LeaveLobby()
    {
        var playerId = _httpContext.User.Claims.FirstOrDefault(w => w.Type == "id").Value;
        var userName = _httpContext.User.Identity.Name;
        _httpContext.Request.Cookies.TryGetValue("cur_lobby", out var lobbyId);
        
        var response = _lobbyService.DisconnectUser(new Guid(playerId), Guid.Parse(lobbyId));
        // Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyIdString);
        Clients.Group(lobbyId).SendAsync("LeaveLobby", userName);
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