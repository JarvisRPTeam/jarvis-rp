using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GameDb.Repository.Migrations
{
    public partial class InventoryFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_Players_OwnerId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Players_PlayerId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_PlayerId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_Players_OwnerId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Players_PlayerId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Players_OwnerId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Inventories");

            migrationBuilder.AddColumn<long>(
                name: "InventoryId",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Inventories",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "InventoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_Players_OwnerId",
                table: "Garages",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Inventories_InventoryId",
                table: "Players",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments",
                column: "AdminId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_PlayerId",
                table: "Punishments",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_Players_OwnerId",
                table: "RealEstates",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Players_PlayerId",
                table: "Residences",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Players_OwnerId",
                table: "Vehicles",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garages_Players_OwnerId",
                table: "Garages");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Inventories_InventoryId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_PlayerId",
                table: "Punishments");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_Players_OwnerId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Players_PlayerId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Players_OwnerId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Inventories");

            migrationBuilder.AddColumn<long>(
                name: "PlayerId",
                table: "Inventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garages_Players_OwnerId",
                table: "Garages",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Players_PlayerId",
                table: "Inventories",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments",
                column: "AdminId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_PlayerId",
                table: "Punishments",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_Players_OwnerId",
                table: "RealEstates",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Players_PlayerId",
                table: "Residences",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Players_OwnerId",
                table: "Vehicles",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
