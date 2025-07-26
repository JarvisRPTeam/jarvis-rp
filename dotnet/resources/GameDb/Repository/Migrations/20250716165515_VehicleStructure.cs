using GameDb.Domain.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameDb.Repository.Migrations
{
    public partial class VehicleStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<VehicleColorModel>(
                name: "Color",
                table: "Vehicles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelledById",
                table: "Punishments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Punishments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_CancelledById",
                table: "Punishments",
                column: "CancelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments",
                column: "CancelledById",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_CancelledById",
                table: "Punishments");

            migrationBuilder.DropIndex(
                name: "IX_Punishments_CancelledById",
                table: "Punishments");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CancelledById",
                table: "Punishments");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Punishments");
        }
    }
}
