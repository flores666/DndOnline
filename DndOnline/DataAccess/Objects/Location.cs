namespace DndOnline.DataAccess.Objects;

public class Location
{
    public Guid EntityId { get; set; }
    public Entity Entity { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double? X { get; set; }
    public double? Y { get; set; }
}