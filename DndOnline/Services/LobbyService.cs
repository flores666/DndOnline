using DndOnline.DataAccess;
using DndOnline.DataAccess.Objects;
using DndOnline.Models;
using DndOnline.Services.Interfaces;

namespace DndOnline.Services;

public class LobbyService : ILobbyService
{
    private readonly DndAppDbContext _db;
    private readonly HttpContext _httpContext;
    
    public LobbyService(DndAppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _db = context;
        _httpContext = httpContextAccessor.HttpContext;
    }
    
    public Lobby CreateLobby(string name, int maxPlayers = 6)
    {
        var master = _httpContext.User.Identity.Name;
        var lobby = new Lobby()
        {
            Name = name,
            Master = master ?? "", 
            Players = new List<User>(),
            MaxPlayers = maxPlayers
        };
        
        _db.Lobbies.Add(lobby);
        _db.SaveChanges();
        return lobby;
    }

    public ResponseModel DeleteLobby()
    {
        throw new NotImplementedException();
    }

    public Lobby GetLobby()
    {
        throw new NotImplementedException();
    }

    public ResponseModel ConnectUser(string userName)
    {
        throw new NotImplementedException();
    }
}