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
            Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
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
            .Include(w => w.Entities)
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

    public async Task<ResponseModel> AddEntityAsync(Guid lobbyId, EntityViewModel model)
    {
        var response = new ResponseModel();
        var userId = new Guid(_httpContext.User.Claims.FirstOrDefault(f => f.Type == "id").Value);

        var result = await _fIleService.SaveAsync(model.File, "entity");
        if (!result.IsSuccess) return result;

        var data = result.Data as FileModel;
        var path = data.RelativePath;
        var picId = Guid.NewGuid();

        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Picture = _db.Pictures.FirstOrDefault(w => w.Path == path) ??
                      new Picture
                      {
                          Id = picId,
                          Path = path,
                          UserId = userId
                      },
            PictureId = picId,
            UserId = userId
        };

        _db.Entities.Add(entity);
        var lobby = _db.Lobbies
            .Include(lobby => lobby.Entities)
            .FirstOrDefault(w => w.Id == lobbyId);
        lobby?.Entities.Add(entity);

        _db.Lobbies.Update(lobby);
        var res = await _db.SaveChangesAsync();
        if (res > 0)
        {
            model.FilePath = path;
            response.SetSuccess(model);
        }

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
        var locId = Guid.NewGuid();
        var location = new Location()
        {
            Id = locId,
            Name = model.Name,
            Path = path,
            UserId = userId
        };

        _db.Locations.Add(location);
        var lobby = _db.Lobbies
            .Include(lobby => lobby.Maps)
            .FirstOrDefault(w => w.Id == lobbyId);
        lobby?.Maps.Add(location);
        _db.Lobbies.Update(lobby);

        var res = await _db.SaveChangesAsync();
        if (res > 0)
        {
            model.FilePath = path;
            response.SetSuccess(model);
        }

        return response;
    }

    public List<EntityViewModel> GetEntities(Guid lobbyId)
    {
        var entities = _db.Lobbies
            .Include(lobby => lobby.Entities)
            .ThenInclude(entity => entity.Picture)
            .FirstOrDefault(w => w.Id == lobbyId)?
            .Entities;

        return entities?.Select(s => new EntityViewModel
        {
            Name = s.Name,
            Description = s.Description,
            FilePath = s.Picture.Path
        }).ToList() ?? new List<EntityViewModel>();
    }

    public List<MapViewModel> GetMaps(Guid lobbyId)
    {
        var maps = _db.Lobbies
            .Include(lobby => lobby.Maps)
            .FirstOrDefault(w => w.Id == lobbyId)?.Maps;

        return maps?.Select(s => new MapViewModel
            {
                Name = s.Name,
                FilePath = s.Path
            })
            .ToList() ?? new List<MapViewModel>();
    }

    /// <summary>
    /// Сохранить состояние сцены
    /// </summary>
    /// <param name="id">id лобби</param>
    /// <param name="json"></param>
    /// <returns></returns>
    public async Task<ResponseModel> SaveSceneAsync(Guid id, string json)
    {
        var response = new ResponseModel();

        var scene = _db.Scenes
            .FirstOrDefault(w => w.LobbyId == id);

        if (scene != null)
        {
            scene.Data = json;
            _db.Scenes.Update(scene);
        }
        else
        {
            await _db.Scenes.AddAsync(new Scene
            {
                Id = Guid.NewGuid(),
                LobbyId = id,
                Data = json
            });
        }

        var res = await _db.SaveChangesAsync();

        if (res > 0) response.SetSuccess();
        
        return response;
    }
}