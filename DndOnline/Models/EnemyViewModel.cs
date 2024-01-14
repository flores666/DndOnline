namespace DndOnline.Models;

public class EnemyViewModel
{
    public string Name { get; set; }
    public string FilePath =>  "Content/Enemies/" + (Name ?? "default.png");
    public FormFile? File { get; set; } 
}