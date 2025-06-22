using GameDb.Domain.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameDb.Repository.Migrations
{
    public partial class MechanicsRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Heading",
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
                name: "Heading",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PositionZ",
                table: "Players");

            migrationBuilder.AddColumn<PositionModel>(
                name: "Position",
                table: "Vehicles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<PositionModel>(
                name: "SpawnPoint",
                table: "RealEstates",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "Punishments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "BankCardPIN",
                table: "Players",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "BankCardNumber",
                table: "Players",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "BankBalance",
                table: "Players",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<PositionModel>(
                name: "Position",
                table: "Players",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_AdminId",
                table: "Punishments",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments",
                column: "AdminId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Punishments_Players_AdminId",
                table: "Punishments");

            migrationBuilder.DropIndex(
                name: "IX_Punishments_AdminId",
                table: "Punishments");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SpawnPoint",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Punishments");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Players");

            migrationBuilder.AddColumn<float>(
                name: "Heading",
                table: "Vehicles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionX",
                table: "Vehicles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionY",
                table: "Vehicles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionZ",
                table: "Vehicles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointHeading",
                table: "RealEstates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointX",
                table: "RealEstates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointY",
                table: "RealEstates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SpawnPointZ",
                table: "RealEstates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<long>(
                name: "BankCardPIN",
                table: "Players",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BankCardNumber",
                table: "Players",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BankBalance",
                table: "Players",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Heading",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionX",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionY",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PositionZ",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
