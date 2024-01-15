namespace DndOnline.Models;

public class GameItemViewModel
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string Description { get; set; }
    public FormFile? File { get; set; }
}