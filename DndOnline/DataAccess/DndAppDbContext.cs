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

        #region scene_triggers

        var sql = @"create function set_defaults_to_scene() returns trigger
                    language plpgsql
                    as
                    $$BEGIN
                        Update scenes
                        Set sort = (Select count(*) From scenes Where lobby_id = new.lobby_id)
                        Where id = new.id;

                        Return new;
                    END;
                    $$;

                    alter function set_defaults_to_scene() owner to postgres;

                    create trigger set_defaults_to_scene_trigger
                    after insert
                    on scenes
                    for each row
                    execute procedure set_defaults_to_scene();";

        Database.ExecuteSqlRaw(sql);

        sql = @"create function after_delete_scene_normalize_sort() returns trigger
                language plpgsql
                    as
                    $$
                    BEGIN
                      -- Приводим сортировку в порядок после удаления элемента
                      UPDATE scenes AS bx
                      SET sort = t.N_Row
                      FROM (
                        SELECT b.id, ROW_NUMBER() OVER(ORDER BY sort ASC) AS N_Row
                        FROM scenes b
                        WHERE b.id <> OLD.id AND b.lobby_id = OLD.lobby_id
                      ) t
                      WHERE t.id = bx.id;

                      RETURN NULL;
                    END;
                    $$;

                    alter function after_delete_scene_normalize_sort() owner to postgres;

                    create trigger after_delete_normalize_sort_trigger
                        after delete
                        on scenes
                        for each row
                        execute procedure after_delete_scene_normalize_sort();";

        Database.ExecuteSqlRaw(sql);

        sql = @"
                    create function update_scene_sort_order() returns trigger
                    language plpgsql
                    as
                    $$BEGIN
                        if (new.sort > old.sort)
                        then
                            Update scenes
                                Set sort = sort - 1
                            Where id <> new.id
                                and lobby_id = new.lobby_id
                                and sort > old.sort
                                and sort <= new.sort;
                        else
                            Update scenes
                                Set sort = sort + 1
                            Where id <> new.id
                                and lobby_id = new.lobby_id
                                and sort = new.sort;
                        end if;

                        RETURN NULL;
                    END;
                    $$;

                    alter function update_scene_sort_order() owner to postgres;

                    -- auto-generated definition
                    create trigger update_scene_sort_order_trigger
                        after UPDATE
                        on scenes
                        for each row
                        execute procedure update_scene_sort_order();";

        Database.ExecuteSqlRaw(sql);

        #endregion
    }
}