﻿using DndOnline.DataAccess;
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

    public ResponseModel CreateLobby(LobbyFormViewModel model)
    {
        var response = new ResponseModel();
        var masterId = _httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value;

        var lobby = new Lobby()
        {
            Id = model.Id == Guid.Empty ? new Guid() : model.Id,
            Name = model.Name,
            MasterId = new Guid(masterId),
            MaxPlayers = model.MaxPlayers,
            Description = model.Description,
        };

        if (model.Id != Guid.Empty)
        {
            var existingLobby = GetLobby(model.Id);
            if (existingLobby != null)
            {
                if (existingLobby.StatusId is LobbyStatusType.WaitingForPlayers
                    or LobbyStatusType.ReadyToStart
                    or LobbyStatusType.Paused)
                {
                    response.Message = "Лобби с таким названием уже существует. Придумайте новое";
                    return response;
                }

                _db.Entry(existingLobby).State = EntityState.Detached;
            }

            lobby.StatusId = LobbyStatusType.WaitingForPlayers;
            _db.Lobbies.Update(lobby);
        }
        else
        {
            _db.Lobbies.Add(lobby);
        }

        var res = _db.SaveChanges();
        if (res > 0) response.SetSuccess(lobby);

        return response;
    }

    public ResponseModel DeleteLobby()
    {
        throw new NotImplementedException();
    }

    public Lobby GetLobby(string name)
    {
        return _db.Lobbies.FirstOrDefault(f => f.Name == name);
    }

    public Lobby GetLobby(Guid id)
    {
        return _db.Lobbies.FirstOrDefault(f => f.Id == id);
    }

    public Lobby GetLobby(Guid userId, LobbyStatusType status)
    {
        return _db.Lobbies
            .FirstOrDefault(w => w.MasterId == userId && w.Status.Status == status);
    }

    public Lobby GetLobbyFull(Guid id)
    {
        return _db.Lobbies
            .Include(w => w.Players)
            .Include(w => w.Status)
            .Include(w => w.Characters)
            .Include(w => w.Creatues)
            .Include(w => w.Items)
            .Include(w => w.Maps)
            .FirstOrDefault(w => w.Id == id);
    }

    public List<Lobby> GetLobbies(int page = 1, int pageSize = 20, string input = null)
    {
        var query = _db.Lobbies.AsQueryable();

        query = query.Where(w => w.StatusId == LobbyStatusType.WaitingForPlayers ||
                                 w.StatusId == LobbyStatusType.ReadyToStart ||
                                 w.StatusId == LobbyStatusType.Paused);

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
        var userId = new Guid(_httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = await _fIleService.SaveAsync(model.File, "item");
        if (!result.IsSuccess) return result;

        var data = result.Data as FileModel;
        var path = data.RelativePath;

        var item = new Item
        {
            Id = new Guid(),
            Name = model.Name,
            Description = model.Description,
            RelativePath = path,
            UserId = userId
        };

        _db.ItemPositions.Add(new()
        {
            Item = item,
            LobbyId = lobbyId
        });

        var res = await _db.SaveChangesAsync();

        if (res > 0) response.SetSuccess(new ItemViewModel(item));

        return response;
    }

    public async Task<ResponseModel> AddCreatureAsync(Guid lobbyId, CreatureViewModel model)
    {
        var response = new ResponseModel();
        var userId = new Guid(_httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = await _fIleService.SaveAsync(model.File, "creature");
        if (!result.IsSuccess) return result;

        var data = result.Data as FileModel;
        var path = data.RelativePath;

        var creature = new Creature
        {
            Id = new Guid(),
            Name = model.Name,
            Description = model.Description,
            RelativePath = path,
            UserId = userId
        };

        _db.CreaturePositions.Add(new()
        {
            Creature = creature,
            LobbyId = lobbyId
        });

        var res = await _db.SaveChangesAsync();

        if (res > 0) response.SetSuccess(new CreatureViewModel(creature));

        return response;
    }

    public async Task<ResponseModel> AddCharacterAsync(Guid lobbyId, CharacterViewModel model)
    {
        var response = new ResponseModel();
        var userId = new Guid(_httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = await _fIleService.SaveAsync(model.File, "character");
        if (!result.IsSuccess) return result;

        var data = result.Data as FileModel;
        var path = data.RelativePath;

        var character = new Character
        {
            Id = new Guid(),
            Name = model.Name,
            Description = model.Description,
            RelativePath = path,
            UserId = userId
        };

        _db.CharacterPositions.Add(new()
        {
            Character = character,
            LobbyId = lobbyId
        });

        var res = await _db.SaveChangesAsync();

        if (res > 0) response.SetSuccess(new CharacterViewModel(character));

        return response;
    }

    public async Task<ResponseModel> AddMapAsync(Guid lobbyId, MapViewModel model)
    {
        var response = new ResponseModel();
        var userId = new Guid(_httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = await _fIleService.SaveAsync(model.File, "map");
        if (!result.IsSuccess) return result;

        var data = result.Data as FileModel;
        var path = data.RelativePath;

        var map = new Map
        {
            Id = new Guid(),
            Name = model.Name,
            Description = model.Description,
            RelativePath = path,
            UserId = userId
        };

        _db.LobbyMaps.Add(new()
        {
            LobbyId = map.Id,
            Map = map,
        });

        var res = await _db.SaveChangesAsync();

        if (res > 0) response.SetSuccess(new MapViewModel(map));

        return response;
    }

    public List<Item> GetItems(Guid lobbyId)
    {
        return _db.ItemPositions
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
        return _db.CreaturePositions
            .Where(w => w.LobbyId == lobbyId)
            .Select(s => new Creature()
            {
                Id = s.Creature.Id,
                Name = s.Creature.Name,
                Description = s.Creature.Description,
                RelativePath = s.Creature.RelativePath
            })
            .ToList();
    }

    public List<Character> GetCharacters(Guid lobbyId)
    {
        return _db.CharacterPositions
            .Where(w => w.LobbyId == lobbyId)
            .Select(s => new Character()
            {
                Id = s.Character.Id,
                Name = s.Character.Name,
                Description = s.Character.Description,
                RelativePath = s.Character.RelativePath
            })
            .ToList();
    }

    public List<Map> GetMaps(Guid lobbyId)
    {
        return _db.LobbyMaps
            .Where(w => w.LobbyId == lobbyId)
            .Select(s => new Map()
            {
                Id = s.Map.Id,
                Name = s.Map.Name,
                Description = s.Map.Description,
                RelativePath = s.Map.RelativePath
            })
            .ToList();
    }
}