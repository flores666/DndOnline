using DndOnline.DataAccess;
using DndOnline.DataAccess.Objects;
using DndOnline.Models;
using DndOnline.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    
    public Lobby CreateLobby(LobbyFormViewModel model)
    {
        var master = _httpContext.User.Identity.Name;
        var lobby = new Lobby()
        {
            Name = model.Name,
            Master = master,
            MaxPlayers = model.MaxPlayers
        };
        
        _db.Lobbies.Add(lobby);
        _db.SaveChanges();
        return lobby;
    }

    public ResponseModel DeleteLobby()
    {
        throw new NotImplementedException();
    }

    public Lobby GetLobby(string name)
    {
        return _db.Lobbies.Include(i => i.Players).FirstOrDefault(f => f.Name == name);
    }

    public Lobby GetLobby(Guid id)
    {
        return _db.Lobbies.Include(i => i.Players).FirstOrDefault(f => f.Id == id);
    }

    public List<Lobby> GetLobbies(int curPage = 1, int pageSize = 20)
    {
        return _db.Lobbies
            .Skip((curPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public ResponseModel ConnectUser(string userName)
    {
        throw new NotImplementedException();
    }

    public ResponseModel ConnectUser(Guid userId, Lobby lobby)
    {
        var response = new ResponseModel();
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (lobby.Players.Any(a => a.Id == userId)) return response;
        
        lobby.Players.Add(user);
        var result = _db.SaveChanges();
        
        if (result > 0) response.SetSeccess();
        return response;
    }
}