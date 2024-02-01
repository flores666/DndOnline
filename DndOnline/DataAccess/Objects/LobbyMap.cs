namespace DndOnline.DataAccess.Objects;

public class LobbyMap
{
    public Guid MapId { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public Map Map { get; set; }
}