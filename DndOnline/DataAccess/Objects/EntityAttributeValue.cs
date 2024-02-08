namespace DndOnline.DataAccess.Objects;

public class EntityAttributeValue
{
    public Guid TypeId { get; set; }
    public EntityType Type { get; set; }
    public Guid AttributeId { get; set; }
    public EntityAttribute Attribute { get; set; }
    public string Value { get; set; }
}