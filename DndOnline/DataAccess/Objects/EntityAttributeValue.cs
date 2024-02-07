namespace DndOnline.DataAccess.Objects;

public class EntityAttributeValue
{
    public Guid EntityTypeId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid AttributeId { get; set; }
    public EntityAttribute Attribute { get; set; }
    public string Value { get; set; }
}