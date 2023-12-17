using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(25)", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Users_RefreshTokens_RefreshTokenToken",
                        column: x => x.RefreshTokenToken,
                        principalTable: "RefreshTokens",
                        principalColumn: "Token");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshTokenToken",
                table: "Users",
                column: "RefreshTokenToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
