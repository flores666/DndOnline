namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public string Master { get; set; }
    public List<User> Players { get; set; } 
    public List<Enemy> Enemies { get; set; } 
    public List<Character> Characters { get; set; } 
    public List<GameItem> GameItems { get; set; } 
    public List<Map> Maps { get; set; } 

    public Lobby()
    {
        MaxPlayers = 6;
        Status = new LobbyStatus();
        Players = new List<User>();
        Enemies = new List<Enemy>();
        Characters = new List<Character>();
        GameItems = new List<GameItem>();
        Maps = new List<Map>();
    }
}