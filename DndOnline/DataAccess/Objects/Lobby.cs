using AuthService.DataAccess.Objects;

namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public string Master { get; set; }
    public List<User> Players { get; set; } 

    public Lobby()
    {
        Status = new LobbyStatus();
        MaxPlayers = 6;
        Players = new List<User>();
    }
}