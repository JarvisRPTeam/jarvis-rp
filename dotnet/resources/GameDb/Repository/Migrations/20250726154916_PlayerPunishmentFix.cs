using Microsoft.EntityFrameworkCore.Migrations;

namespace GameDb.Repository.Migrations
{
    public partial class PlayerPunishmentFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments");

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments",
                column: "AdminId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments",
                column: "CancelledById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments");

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments",
                column: "AdminId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments",
                column: "CancelledById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
