using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KickStats.Migrations
{
    /// <inheritdoc />
    public partial class PlayerList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player1Id",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player2Id",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_Player1Id",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_Player2Id",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Player2Id",
                table: "Teams");

            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => new { x.PlayerId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_TeamPlayers_AspNetUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_TeamId",
                table: "TeamPlayers",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamPlayers");

            migrationBuilder.AddColumn<Guid>(
                name: "Player1Id",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Player2Id",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Player1Id",
                table: "Teams",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Player2Id",
                table: "Teams",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_Player1Id",
                table: "Teams",
                column: "Player1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_Player2Id",
                table: "Teams",
                column: "Player2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
