namespace DndOnline.DataAccess.Objects;

public class CreatureLobby
{
    public Guid CreatureId { get; set; }
    public Creature Creature  { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; } = 0.0;
    public double Y { get; set; } = 0.0;
}