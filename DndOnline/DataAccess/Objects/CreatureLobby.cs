namespace DndOnline.DataAccess.Objects;

public class CreatureLobby
{
    public Guid EnemyId { get; set; }
    public Creature Creature  { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}