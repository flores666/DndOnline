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
    private readonly IFileService _fIleService;

    public LobbyService(DndAppDbContext context, IHttpContextAccessor httpContextAccessor,
        IFileService fs)
    {
        _db = context;
        _httpContext = httpContextAccessor.HttpContext;
        _fIleService = fs;
    }

    public Lobby CreateLobby(LobbyFormViewModel model)
    {
        var masterId = _httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value;
        var lobby = new Lobby()
        {
            Id = model.Id == Guid.Empty ? new Guid() : model.Id,
            Name = model.Name,
            MasterId = new Guid(masterId),
            MaxPlayers = model.MaxPlayers,
            Description = model.Description,
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

    public Lobby GetLobby(Guid userId, LobbyStatusType status)
    {
        return _db.Lobbies
            .FirstOrDefault(w => w.MasterId == userId && w.Status.Status == status);
    }

    public List<Lobby> GetLobbies(int page = 1, int pageSize = 20)
    {
        return _db.Lobbies
            .Include(i => i.Players)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<Lobby> GetLobbies(string input, int page = 1, int pageSize = 20)
    {
        var query = _db.Lobbies.AsQueryable();

        if (!string.IsNullOrEmpty(input))
            query = query
                .Where(w => w.Name.ToLower().Contains(input.ToLower()));

        return query
            .Include(i => i.Players)
            .Skip((page - 1) * pageSize)
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

        if (result > 0) response.SetSuccess(user);
        return response;
    }

    public ResponseModel DisconnectUser(Guid userId, Lobby lobby)
    {
        throw new NotImplementedException();
    }

    public ResponseModel DisconnectUser(Guid userId, Guid lobbyId)
    {
        var response = new ResponseModel();
        var lobby = _db.Lobbies
            .Include(lobby => lobby.Players)
            .Include(lobby => lobby.Status)
            .FirstOrDefault(f => f.Id == lobbyId);

        var toRemove = lobby.Players.FirstOrDefault(f => f.Id == userId);

        if (toRemove != null)
        {
            var isRemoved = lobby.Players.Remove(toRemove);
            if (isRemoved)
            {
                /*if (lobby.Players.Count == 0 && lobby.Status.Status == LobbyStatusType.WaitingForPlayers)
                {
                    _db.Lobbies.Remove(lobby);
                }
                else
                {
                    _db.Lobbies.Update(lobby);
                }*/
                _db.Lobbies.Update(lobby);

                var saved = _db.SaveChanges();
                if (saved > 0)
                {
                    response.SetSuccess();
                    response.Data = toRemove;
                }
            }
        }

        return response;
    }

    public async Task<ResponseModel> AddItemAsync(Guid lobbyId, ItemViewModel model)
    {
        var response = new ResponseModel();

        var result = await _fIleService.SaveAsync(model.File, "item");
        if (!result.IsSuccess) return result;
        
        var data = result.Data as FileModel;
        var path = data.RelativePath;

        var item = new Item
        {
            Id = new Guid(),
            Name = model.Name,
            Description = model.Description,
            RelativePath = path
        };
        
        _db.Items.Add(item);
        var res = await _db.SaveChangesAsync();

        _db.ItemLobby.Add(new ()
        {
            ItemId = item.Id,
            LobbyId = lobbyId 
        });

        res += await _db.SaveChangesAsync();
        
        if (res > 0) response.SetSuccess(new ItemViewModel(item));

        return response;
    }

    public async Task<ResponseModel> AddCreature(Guid lobbyId, CreatureViewModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseModel> AddCharacter(Guid lobbyId, CharacterViewModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseModel> AddMap(Guid lobbyId, MapViewModel model)
    {
        throw new NotImplementedException();
    }

    public List<Item> GetItems(Guid lobbyId)
    {
        return _db.ItemLobby
            .Where(w => w.LobbyId == lobbyId)
            .Select(s => new Item()
            {
                Id = s.Item.Id,
                Name = s.Item.Name,
                Description = s.Item.Description,
                RelativePath = s.Item.RelativePath
            })
            .ToList();
    }

    public List<Creature> GetCreatures(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public List<Character> GetCharacters(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public List<Map> GetMaps(Guid lobbyId)
    {
        throw new NotImplementedException();
    }
}