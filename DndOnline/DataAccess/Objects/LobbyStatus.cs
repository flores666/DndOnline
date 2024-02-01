using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

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

[PrimaryKey("Status")]
public class LobbyStatus
{
    public LobbyStatusType Status { get; set; } = LobbyStatusType.Draft;
    public string Name { get; set; }
    
    public override string ToString()
    {
        var pattern = @"(?<=\p{Ll})(?=\p{Lu})";
        var result = Regex.Replace(Status.ToString(), pattern, " ");
        return result;
    }
}