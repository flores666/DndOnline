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
    public DbSet<EntityLocation> EntityLocations { get; set; }
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<EntityAttribute> EntityAttributes { get; set; }
    public DbSet<EntityAttributeValue> EntityAttributeValues { get; set; }
    public DbSet<EntityTypePicture> EntityTypePictures { get; set; }

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
                    .WithMany(t => t.EntityLocations)
                    .HasForeignKey(pt => pt.EntityId),
                j =>
                {
                    j.Property(pt => pt.X).HasDefaultValue(0.0);
                    j.Property(pt => pt.Y).HasDefaultValue(0.0);
                    j.HasKey(t => new { t.EntityId, t.LobbyId });
                    j.ToTable("EntityLocations");
                });

        #endregion

        #region EntityAttributeValue

        // Устанавливаем составной ключ для таблицы связи EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>()
            .HasKey(eav => new { eav.EntityTypeId, eav.AttributeId });

        // Определяем связь между EntityType и EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>()
            .HasOne(eav => eav.EntityType)
            .WithMany(et => et.EntityAttributeValues)
            .HasForeignKey(eav => eav.EntityTypeId);

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

        // Генерация идентификаторов
        var entityId1 = Guid.NewGuid();
        var entityId2 = Guid.NewGuid();
        var entityId3 = Guid.NewGuid();
        var entityId4 = Guid.NewGuid();
        var entityId5 = Guid.NewGuid();

        var humanId = Guid.NewGuid();
        var elfId = Guid.NewGuid();
        var dwarfId = Guid.NewGuid();
        var orcId = Guid.NewGuid();
        var goblinId = Guid.NewGuid();

        var strengthId = Guid.NewGuid();
        var dexterityId = Guid.NewGuid();
        var constitutionId = Guid.NewGuid();
        var intelligenceId = Guid.NewGuid();
        var wisdomId = Guid.NewGuid();

        modelBuilder.Entity<EntityType>().HasData(
            new EntityType { Id = humanId, Name = "Human" },
            new EntityType { Id = elfId, Name = "Elf" },
            new EntityType { Id = dwarfId, Name = "Dwarf" },
            new EntityType { Id = orcId, Name = "Orc" },
            new EntityType { Id = goblinId, Name = "Goblin" }
        );

        modelBuilder.Entity<EntityAttribute>().HasData(
            new EntityAttribute { Id = strengthId, Name = "Strength" },
            new EntityAttribute { Id = dexterityId, Name = "Dexterity" },
            new EntityAttribute { Id = constitutionId, Name = "Constitution" },
            new EntityAttribute { Id = intelligenceId, Name = "Intelligence" },
            new EntityAttribute { Id = wisdomId, Name = "Wisdom" }
        );

        modelBuilder.Entity<EntityAttributeValue>().HasData(
            new EntityAttributeValue { EntityTypeId = humanId, AttributeId = strengthId, Value = "10" },
            new EntityAttributeValue { EntityTypeId = humanId, AttributeId = dexterityId, Value = "12" },
            new EntityAttributeValue { EntityTypeId = humanId, AttributeId = constitutionId, Value = "11" },
            new EntityAttributeValue { EntityTypeId = humanId, AttributeId = intelligenceId, Value = "14" },
            new EntityAttributeValue { EntityTypeId = humanId, AttributeId = wisdomId, Value = "13" }
        );

        modelBuilder.Entity<Entity>().HasData(
            new Entity { Id = entityId1, Name = "Человек", Description = "Описание человека", TypeId = humanId },
            new Entity { Id = entityId2, Name = "Эльф", Description = "Описание эльфа", TypeId = elfId },
            new Entity { Id = entityId3, Name = "Дварф", Description = "Описание дварфа", TypeId = dwarfId },
            new Entity { Id = entityId4, Name = "Орк", Description = "Описание орка", TypeId = orcId },
            new Entity { Id = entityId5, Name = "Гоблин", Description = "Описание гоблина", TypeId = goblinId }
        );

        // Генерация идентификаторов для типов сущностей
        var itemType = Guid.NewGuid();

        // Генерация идентификаторов для атрибутов
        var weightAttribute = Guid.NewGuid();
        var durabilityAttribute = Guid.NewGuid();
        var damageAttribute = Guid.NewGuid();
        var priceAttribute = Guid.NewGuid();

        // Заполнение таблицы EntityType
        modelBuilder.Entity<EntityType>().HasData(
            new EntityType { Id = itemType, Name = "Предмет" }
        );

        // Заполнение таблицы EntityAttribute
        modelBuilder.Entity<EntityAttribute>().HasData(
            new EntityAttribute { Id = weightAttribute, Name = "Вес" },
            new EntityAttribute { Id = durabilityAttribute, Name = "Прочность" },
            new EntityAttribute { Id = damageAttribute, Name = "Урон" },
            new EntityAttribute { Id = priceAttribute, Name = "Цена" }
        );

        // Заполнение таблицы EntityAttributeValue
        modelBuilder.Entity<EntityAttributeValue>().HasData(
            // Значения атрибутов для типа "Предмет"
            new EntityAttributeValue { EntityTypeId = itemType, AttributeId = weightAttribute, Value = "5 кг" },
            new EntityAttributeValue { EntityTypeId = itemType, AttributeId = durabilityAttribute, Value = "Средняя" },
            new EntityAttributeValue { EntityTypeId = itemType, AttributeId = priceAttribute, Value = "100 золотых" }
        );

        modelBuilder.Entity<Entity>().HasData(
            new Entity { Id = Guid.NewGuid(), Name = "Меч", Description = "Описание меча", TypeId = itemType },
            new Entity { Id = Guid.NewGuid(), Name = "Щит", Description = "Описание щита", TypeId = itemType },
            new Entity { Id = Guid.NewGuid(), Name = "Посох", Description = "Описание посоха", TypeId = itemType },
            new Entity { Id = Guid.NewGuid(), Name = "Лук", Description = "Описание лука", TypeId = itemType },
            new Entity { Id = Guid.NewGuid(), Name = "Кольцо", Description = "Описание кольца", TypeId = itemType }
        );

        modelBuilder.Entity<EntityTypePicture>().HasData(
            new EntityTypePicture { EntityTypeId = itemType, Path = "/content/default.png"},
            new EntityTypePicture { EntityTypeId = humanId, Path = "/content/default.png"},
            new EntityTypePicture { EntityTypeId = elfId, Path = "/content/default.png"},
            new EntityTypePicture { EntityTypeId = dwarfId, Path = "/content/default.png"},
            new EntityTypePicture { EntityTypeId = orcId, Path = "/content/default.png"},
            new EntityTypePicture { EntityTypeId = goblinId, Path = "/content/default.png"}
        );
        
        #endregion
    }
}