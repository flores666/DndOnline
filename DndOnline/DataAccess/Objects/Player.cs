﻿using AuthService.DataAccess.Objects;

namespace DndOnline.DataAccess.Objects;

public class Player : User
{
    public GameRole Role { get; set; }
    public List<Lobby> Lobbies { get; set; } = new List<Lobby>();
}