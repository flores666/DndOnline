namespace DndOnline.Models;

public class GameItemViewModel
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public IFormFile File { get; set; }
}