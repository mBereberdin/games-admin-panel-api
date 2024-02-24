using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности.", defaultValueSql: "gen_random_uuid()"),
                    Email = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "Электронная почта."),
                    Nickname = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Имя пользователя.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности.", defaultValueSql: "gen_random_uuid()"),
                    EncryptedValue = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "Зашифрованное значение."),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Внешний ключ записи пользователя.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passwords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_UserId",
                table: "Passwords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Nickname",
                table: "Users",
                column: "Nickname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
