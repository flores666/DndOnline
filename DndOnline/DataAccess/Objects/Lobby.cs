namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public string Master { get; set; }
    public List<User> Players { get; set; } 
    public List<Enemy> Enemies = new List<Enemy>();
    public List<Character> Characters = new List<Character>();
    public List<GameItem> GameItems = new List<GameItem>();
    public Map? Map;

    public Lobby()
    {
        Status = new LobbyStatus();
        MaxPlayers = 6;
        Players = new List<User>();
    }
}