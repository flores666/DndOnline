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
    public DbSet<Scene> Scenes { get; set; }

    public DndAppDbContext()
    {
    }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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