using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects;

public class Scene
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    [ForeignKey("Lobby")]
    public Guid LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    /// <summary>
    /// json сцены
    /// </summary>
    public string? Data { get; set; }
    /// <summary>
    /// порядковый номер
    /// </summary>
    public int Sort { get; set; }
}