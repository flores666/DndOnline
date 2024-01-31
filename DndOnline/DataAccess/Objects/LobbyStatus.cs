using System.Text.RegularExpressions;

namespace DndOnline.DataAccess.Objects;

public enum LobbyStatusType
{
    Draft,
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
    public LobbyStatusType Status { get; set; } = LobbyStatusType.Draft;

    public override string ToString()
    {
        var pattern = @"(?<=\p{Ll})(?=\p{Lu})";
        var result = Regex.Replace(Status.ToString(), pattern, " ");
        return result;
    }
}