namespace DndOnline.Models;

public class CharacterViewModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public FormFile File { get; set; }
}