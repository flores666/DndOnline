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
    public DbSet<Location> Locations { get; set; }
    public DbSet<Picture> Pictures { get; set; }

    public DndAppDbContext()
    {
    }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Locations

        modelBuilder
            .Entity<Entity>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Entities)
            .UsingEntity<Location>(
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

        #region MapsInLobby

        modelBuilder
            .Entity<Map>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Maps)
            .UsingEntity<LobbyMap>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.LobbyMaps)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Map)
                    .WithMany(t => t.LobbyMaps)
                    .HasForeignKey(pt => pt.MapId),
                j =>
                {
                    j.HasKey(t => new { t.MapId, t.LobbyId });
                    j.ToTable("LobbyMaps");
                });

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
    }
}