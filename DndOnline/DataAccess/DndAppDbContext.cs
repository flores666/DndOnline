using DndOnline.DataAccess.Objects;
using Microsoft.EntityFrameworkCore;

namespace DndOnline.DataAccess;

public class DndAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<LobbyStatus> LobbyStatuses { get; set; }

    public DbSet<Entity> Entities { get; set; }
    public DbSet<EntityLocation> Locations { get; set; }
    public DbSet<EntityType> Types { get; set; }
    public DbSet<EntitySubType> SubTypes { get; set; }
    public DbSet<EntityAttribute> Attributes { get; set; }
    public DbSet<EntityAttributeValue> AttributeValues { get; set; }
    public DbSet<EntitySubTypePicture> Pictures { get; set; }

    public DndAppDbContext()
    {
    }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region EntityLocations

        modelBuilder
            .Entity<Entity>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Entities)
            .UsingEntity<EntityLocation>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.EntityLocations)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Entity)
                    .WithMany(t => t.Locations)
                    .HasForeignKey(pt => pt.EntityId),
                j =>
                {
                    j.Property(pt => pt.X).HasDefaultValue(0.0);
                    j.Property(pt => pt.Y).HasDefaultValue(0.0);
                    j.HasKey(t => new { t.EntityId, t.LobbyId });
                    j.ToTable("Locations");
                });

        #endregion

        #region EntityAttributeValue

        // Устанавливаем составной ключ для таблицы связи EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>()
            .HasKey(eav => new {EntityTypeId = eav.TypeId, eav.AttributeId });

        // Определяем связь между EntityType и EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>()
            .HasOne(eav => eav.Type)
            .WithMany(et => et.AttributeValues)
            .HasForeignKey(eav => eav.TypeId);

        // Определяем связь между EntityAttribute и EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>()
            .HasOne(eav => eav.Attribute)
            .WithMany()
            .HasForeignKey(eav => eav.AttributeId);

        #endregion

        #region LobbyStatusDefaultValues

        modelBuilder.Entity<LobbyStatus>().HasData(
            new LobbyStatus { Status = LobbyStatusType.Draft, Name = "Черновик" },
            new LobbyStatus { Status = LobbyStatusType.WaitingForPlayers, Name = "Ожидание игроков" },
            new LobbyStatus { Status = LobbyStatusType.ReadyToStart, Name = "Готово к началу" },
            new LobbyStatus { Status = LobbyStatusType.InProgress, Name = "В процессе" },
            new LobbyStatus { Status = LobbyStatusType.Paused, Name = "Приостановлено" },
            new LobbyStatus { Status = LobbyStatusType.Closed, Name = "Закрыто" },
            new LobbyStatus { Status = LobbyStatusType.Completed, Name = "Заверешно" }
        );

        #endregion

        
        #region EntitiesDefaultValues

        var attributeId1 = Guid.NewGuid();
        var attributeId2 = Guid.NewGuid();
        
        // Заполнение таблицы Attributes
        modelBuilder.Entity<EntityAttribute>().HasData(
            new EntityAttribute { Id = attributeId1, Name = "Strength" },
            new EntityAttribute { Id = attributeId2, Name = "Dexterity" }
        );

        var typeId1 = Guid.NewGuid();
        var typeId2 = Guid.NewGuid();
        
        // Заполнение таблицы Types
        modelBuilder.Entity<EntityType>().HasData(
            new EntityType { Id = typeId1, Name = "Creature" },
            new EntityType { Id = typeId2, Name = "Item" }
        );

        // Заполнение таблицы AttributeValues
        modelBuilder.Entity<EntityAttributeValue>().HasData(
            new EntityAttributeValue { TypeId = typeId1, AttributeId = attributeId1, Value = "18" },
            new EntityAttributeValue { TypeId = typeId2, AttributeId = attributeId2, Value = "16" }
        );

        var subTypeId1 = Guid.NewGuid();
        var subTypeId2 = Guid.NewGuid();
        var subTypeId3 = Guid.NewGuid();
        var subTypeId4 = Guid.NewGuid();
        
        // Заполнение таблицы SubTypes
        modelBuilder.Entity<EntitySubType>().HasData(
            new EntitySubType { Id = subTypeId1, TypeId = typeId1, Name = "Warrior", Description = "A strong warrior class." },
            new EntitySubType { Id = subTypeId2, TypeId = typeId1, Name = "Rogue", Description = "A rogue class." },
            new EntitySubType { Id = subTypeId3, TypeId = typeId2, Name = "Gloves", Description = "Cool gloves." },
            new EntitySubType { Id = subTypeId4, TypeId = typeId2, Name = "Helmet", Description = "Cool helmet." }
        );

        // Заполнение таблицы Entities
        modelBuilder.Entity<Entity>().HasData(
            new Entity { Id = Guid.NewGuid(), Name = "A human warrior", Description = "A human warrior", TypeId = typeId1},
            new Entity { Id = Guid.NewGuid(), Name = "An orc brute", Description = "An orc brute", TypeId = typeId1},
            new Entity { Id = Guid.NewGuid(), Name = "Iron Gauntlets.", Description = "An orc brute", TypeId = typeId2},
            new Entity { Id = Guid.NewGuid(), Name = "Siege Helmet.", Description = "An orc brute", TypeId = typeId2}
        );

        // Заполнение таблицы Pictures
        modelBuilder.Entity<EntitySubTypePicture>().HasData(
            new EntitySubTypePicture { SubTypeId = subTypeId1, Path = "/content/default.png" },
            new EntitySubTypePicture { SubTypeId = subTypeId2, Path = "/content/default.png" },
            new EntitySubTypePicture { SubTypeId = subTypeId3, Path = "/content/default.png" },
            new EntitySubTypePicture { SubTypeId = subTypeId4, Path = "/content/default.png" }
        );
        
        #endregion
        
    }
}