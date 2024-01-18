namespace DndOnline.DataAccess.Objects;

public class ItemLobby
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}