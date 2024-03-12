using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRightsAndGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности.", defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "Наименование."),
                    Description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "Описание.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности.", defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "Наименование."),
                    Description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "Описание."),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Внешний ключ записи игры.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rights_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                table: "Games",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rights_GameId",
                table: "Rights",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Rights_Name",
                table: "Rights",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rights");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
