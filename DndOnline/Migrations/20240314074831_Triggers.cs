using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndOnline.Migrations
{
    /// <inheritdoc />
    public partial class Triggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.Sql(sql);

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

            migrationBuilder.Sql(sql);

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

            migrationBuilder.Sql(sql);

            sql =
                "create function newguid() returns uuid\n    language sql\nas\n$$\nselect md5(random()::text || clock_timestamp()::text)::uuid;\n$$;\n\nalter function newguid() owner to postgres;\n\n";

            migrationBuilder.Sql(sql);

            sql =
                "create function update_scene_sort_order() returns trigger\n    language plpgsql\nas\n$$BEGIN\n    if (new.sort > old.sort)\n    then\n        Update scenes\n            Set sort = sort - 1\n        Where id <> new.id\n            and lobby_id = new.lobby_id\n            and sort > old.sort\n            and sort <= new.sort;\n    else\n        Update scenes\n            Set sort = sort + 1\n        Where id <> new.id\n            and lobby_id = new.lobby_id\n            and sort = new.sort;\n    end if;\n\n    RETURN NULL;\nEND;\n$$;\n\nalter function update_scene_sort_order() owner to postgres;\n\n";

            migrationBuilder.Sql(sql);

            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
