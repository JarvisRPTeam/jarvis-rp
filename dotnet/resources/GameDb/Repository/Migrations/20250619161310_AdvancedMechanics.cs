using System;
using System.Collections.Generic;
using GameDb.Domain.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GameDb.Repository.Migrations
{
    public partial class AdvancedMechanics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxVehicleCount",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "IsUsable",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "OnlySlot",
                table: "Items");

            migrationBuilder.AddColumn<float>(
                name: "CurrentFuel",
                table: "Vehicles",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "FuelConsumption",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Heading",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Mileage",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionX",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionY",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionZ",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TankCapacity",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointHeading",
                table: "RealEstates",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointX",
                table: "RealEstates",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointY",
                table: "RealEstates",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointZ",
                table: "RealEstates",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<long>(
                name: "BankBalance",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BankCardNumber",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BankCardPIN",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<byte>(
                name: "Breath",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BreathHoldingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DrivingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Endurance",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "FishingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "FlyingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<float>(
                name: "Heading",
                table: "Players",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<byte>(
                name: "HuntingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<long>(
                name: "JarvisBalance",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PlayedToday",
                table: "Players",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PlayedTotal",
                table: "Players",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "Players",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<byte>(
                name: "ShootingSkill",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<long>(
                name: "SpawnPlaceId",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Stealth",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Strength",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<List<List<InventoryCellModel>>>(
                name: "DefaultCells",
                table: "Items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDurability",
                table: "Items",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ItemUsageModel>(
                name: "Usage",
                table: "Items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<List<InventoryCellModel>>>(
                name: "Cells",
                table: "Inventories",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Garages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(nullable: true),
                    AddressId = table.Column<long>(nullable: false),
                    MaxVehicleCount = table.Column<byte>(nullable: false),
                    VehicleSpawnPoints = table.Column<float[,]>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garages_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Garages_Players_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Punishments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<long>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Timeout = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punishments_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Permissions = table.Column<IEnumerable<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_Nickname",
                table: "Players",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_RoleId",
                table: "Players",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SpawnPlaceId",
                table: "Players",
                column: "SpawnPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_AddressId",
                table: "Garages",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_OwnerId",
                table: "Garages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_PlayerId",
                table: "Punishments",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Roles_RoleId",
                table: "Players",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_RealEstates_SpawnPlaceId",
                table: "Players",
                column: "SpawnPlaceId",
                principalTable: "RealEstates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Roles_RoleId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_RealEstates_SpawnPlaceId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Garages");

            migrationBuilder.DropTable(
                name: "Punishments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Players_Nickname",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_RoleId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_SpawnPlaceId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurrentFuel",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FuelConsumption",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Heading",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PositionZ",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TankCapacity",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SpawnPointHeading",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "SpawnPointX",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "SpawnPointY",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "SpawnPointZ",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "BankBalance",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BankCardNumber",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BankCardPIN",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Breath",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BreathHoldingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DrivingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FishingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FlyingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Heading",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HuntingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "JarvisBalance",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayedToday",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayedTotal",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ShootingSkill",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SpawnPlaceId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Stealth",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DefaultCells",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "HasDurability",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Cells",
                table: "Inventories");

            migrationBuilder.AddColumn<byte>(
                name: "MaxVehicleCount",
                table: "RealEstates",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsable",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "OnlySlot",
                table: "Items",
                type: "smallint",
                nullable: true);
        }
    }
}
