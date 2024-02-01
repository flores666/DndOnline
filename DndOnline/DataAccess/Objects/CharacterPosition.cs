namespace DndOnline.DataAccess.Objects;

public class CharacterPosition
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; } = 0.0;
    public double Y { get; set; } = 0.0;
}