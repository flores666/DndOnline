﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    [ForeignKey("Status")]
    public LobbyStatusType StatusId { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public Guid MasterId { get; set; }
    public List<User> Players { get; set; } 
    public List<Entity> Entities { get; set; }

    public IEnumerable<EntityLocation> EntityLocations { get; set; } = new List<EntityLocation>();

    public Lobby()
    {
        MaxPlayers = 6;
        StatusId = LobbyStatusType.Draft;
        Players = new List<User>();
        Entities = new List<Entity>();
    }
}