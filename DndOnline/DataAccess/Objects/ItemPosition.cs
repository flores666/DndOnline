namespace DndOnline.DataAccess.Objects;

public class ItemPosition
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    public double X { get; set; } = 0.0;
    public double Y { get; set; } = 0.0;
}