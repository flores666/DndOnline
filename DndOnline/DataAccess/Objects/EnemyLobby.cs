namespace DndOnline.DataAccess.Objects;

public class EnemyLobby
{
    public Guid EnemyId { get; set; }
    public Enemy Enemy  { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}