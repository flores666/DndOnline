namespace DndOnline.Models;

public class CharacterViewModel
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public IFormFile File { get; set; }
}