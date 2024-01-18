namespace DndOnline.DataAccess.Objects;

public class CharacterLobby
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}