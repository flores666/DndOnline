namespace DndOnline.DataAccess.Objects;

public enum LobbyStatusType
{
    WaitingForPlayers,
    ReadyToStart,
    InProgress,
    Paused,
    Closed,
    Completed
}

public class LobbyStatus
{
    public Guid Id { get; set; }
    public LobbyStatusType Status { get; set; }
}