namespace DndOnline.DataAccess.Objects;

public class EntityType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public IEnumerable<EntityAttributeValue> AttributeValues { get; set; } = new List<EntityAttributeValue>();
    public IEnumerable<EntitySubType> SubTypes { get; set; } = new List<EntitySubType>();
}