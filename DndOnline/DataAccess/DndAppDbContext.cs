using AuthService.DataAccess.Objects;
using DndOnline.DataAccess.Objects;
using Microsoft.EntityFrameworkCore;

namespace DndOnline.DataAccess;

public class DndAppDbContext : DbContext
{
    public DbSet<Player> Users { get; set; }
    public DbSet<UserRefreshToken> RefreshTokens { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<LobbyStatus> LobbyStatuses { get; set; }
    public DbSet<GamePermission> Permissions { get; set; }
    public DbSet<GameRole> Roles { get; set; }
    
    public DndAppDbContext() { }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Player>()
            .HasMany(p => p.Lobbies)
            .WithMany(l => l.Players)
            .UsingEntity(j => j.ToTable("UserLobby"));
        
        modelBuilder.Entity<Role>()
            .HasMany(p => p.Permissions)
            .WithMany(l => l.Roles)
            .UsingEntity(j => j.ToTable("RolePermission"));
    }
}