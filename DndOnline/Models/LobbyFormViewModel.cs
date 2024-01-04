﻿using System.ComponentModel.DataAnnotations;
using DndOnline.DataAccess.Objects;

namespace DndOnline.Models;

public class LobbyFormViewModel
{
    public Guid Id { get; set; }

    [Required] 
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required] 
    public int MaxPlayers { get; set; } = 6;

    public int PLayersCount { get; set; } = 0;

    [Required] 
    public string Master { get; set; }

    public List<User> Players { get; set; } = new List<User>();

    public List<Enemy> Enemies = new List<Enemy>();
    public List<Character> Characters = new List<Character>();
    public List<GameItem> GameItems = new List<GameItem>();
    public Map? Map;
}