using DndOnline.DataAccess.Objects;
using Microsoft.EntityFrameworkCore;

namespace DndOnline.DataAccess;

public class DndAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<LobbyStatus> LobbyStatuses { get; set; }
    
    public DbSet<Character> Characters { get; set; }
    public DbSet<Enemy> Enemies { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<GameItem> GameItems { get; set; }

    public DndAppDbContext() { }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        /*modelBuilder.Entity<Player>()
            .HasMany(p => p.Lobbies)
            .WithMany(l => l.Players)
            .UsingEntity(j => j.ToTable("UserLobby"));*/
        
        /*modelBuilder.Entity<Role>()
            .HasMany(p => p.Permissions)
            .WithMany(l => l.Roles)
            .UsingEntity(j => j.ToTable("RolePermission"));*/
    }
}