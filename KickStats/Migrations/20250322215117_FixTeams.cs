using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KickStats.Migrations
{
    /// <inheritdoc />
    public partial class FixTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player1Id",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player2Id",
                table: "Teams");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player1Id",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_Player2Id",
                table: "Teams");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_Player1Id",
                table: "Teams",
                column: "Player1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_Player2Id",
                table: "Teams",
                column: "Player2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
