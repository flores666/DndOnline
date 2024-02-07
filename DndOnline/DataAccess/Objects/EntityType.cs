namespace DndOnline.DataAccess.Objects;

public class EntityType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public IEnumerable<EntityAttributeValue> EntityAttributeValues { get; set; } = new List<EntityAttributeValue>();
    public IEnumerable<EntityTypePicture> EntityTypePictures { get; set; } = new List<EntityTypePicture>();
}