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
    public DbSet<Item> Items { get; set; }

    public DbSet<EnemyLobby> EnemyLobby { get; set; }
    public DbSet<CharacterLobby> CharacterLobby { get; set; }
    public DbSet<ItemLobby> ItemLobby { get; set; }
    public DbSet<MapLobby> MapLobby { get; set; }

    public DndAppDbContext()
    {
    }

    public DndAppDbContext(DbContextOptions<DndAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region EnemyLobby

        modelBuilder
            .Entity<Enemy>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Enemies)
            .UsingEntity<EnemyLobby>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.EnemyLobby)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Enemy)
                    .WithMany(t => t.EnemyLobby)
                    .HasForeignKey(pt => pt.EnemyId),
                j =>
                {
                    j.Property(pt => pt.X).HasDefaultValue(0.0);
                    j.Property(pt => pt.Y).HasDefaultValue(0.0);
                    j.HasKey(t => new { t.EnemyId, t.LobbyId });
                    j.ToTable("EnemyLobby");
                });

        #endregion

        #region CharacterLobby

        modelBuilder
            .Entity<Character>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Characters)
            .UsingEntity<CharacterLobby>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.CharacterLobby)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Character)
                    .WithMany(t => t.CharacterLobby)
                    .HasForeignKey(pt => pt.CharacterId),
                j =>
                {
                    j.Property(pt => pt.X).HasDefaultValue(0.0);
                    j.Property(pt => pt.Y).HasDefaultValue(0.0);
                    j.HasKey(t => new { t.CharacterId, t.LobbyId });
                    j.ToTable("CharacterLobby");
                });

        #endregion

        #region ItemLobby

        modelBuilder
            .Entity<Item>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Items)
            .UsingEntity<ItemLobby>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.ItemLobby)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Item)
                    .WithMany(t => t.ItemLobby)
                    .HasForeignKey(pt => pt.ItemId),
                j =>
                {
                    j.Property(pt => pt.X).HasDefaultValue(0.0);
                    j.Property(pt => pt.Y).HasDefaultValue(0.0);
                    j.HasKey(t => new { t.ItemId, t.LobbyId });
                    j.ToTable("ItemLobby");
                });

        #endregion

        #region MapLobby
        
        modelBuilder
            .Entity<Map>()
            .HasMany(c => c.Lobbies)
            .WithMany(s => s.Maps)
            .UsingEntity<MapLobby>(
                j => j
                    .HasOne(pt => pt.Lobby)
                    .WithMany(p => p.MapLobby)
                    .HasForeignKey(pt => pt.LobbyId),
                j => j
                    .HasOne(pt => pt.Map)
                    .WithMany(t => t.MapLobby)
                    .HasForeignKey(pt => pt.MapId),
                j =>
                {
                    j.HasKey(t => new { t.MapId, t.LobbyId });
                    j.ToTable("MapLobby");
                });
        
        #endregion
    }
}