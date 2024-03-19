using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersRights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности.", defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Внешний ключ записи пользователя."),
                    RightId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Внешний ключ записи права.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRights_Rights_RightId",
                        column: x => x.RightId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRights_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersRights_RightId",
                table: "UsersRights",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRights_UserId",
                table: "UsersRights",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersRights");
        }
    }
}
