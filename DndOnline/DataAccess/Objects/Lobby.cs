using AuthService.DataAccess.Objects;

namespace DndOnline.DataAccess.Objects;

public class Lobby
{
    public Guid Id { get; set; }
    public LobbyStatus Status { get; set; }
    public int MaxPlayers { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
}