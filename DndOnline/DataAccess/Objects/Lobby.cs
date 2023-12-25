﻿using AuthService.DataAccess.Objects;

namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public User Master { get; set; }
    public List<User> Players { get; set; } = new List<User>();

    public Lobby()
    {
        Status.Status = LobbyStatusType.WaitingForPlayers;
    }
}