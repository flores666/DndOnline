namespace DndOnline.DataAccess.Objects;

public class Map
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string Path { get; set; }
    public IEnumerable<Lobby> Lobbies { get; set; } = new List<Lobby>();
    public IEnumerable<LobbyMap> LobbyMaps { get; set; } = new List<LobbyMap>();
}