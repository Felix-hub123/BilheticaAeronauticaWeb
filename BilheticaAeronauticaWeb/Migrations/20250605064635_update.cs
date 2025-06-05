using Microsoft.EntityFrameworkCore.Migrations;

namespace BilheticaAeronauticaWeb.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Voos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Passageiros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Lugares",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Bilhetes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Avioes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasDeleted",
                table: "Aeroportos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Voos");

            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Passageiros");

            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Lugares");

            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Bilhetes");

            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Avioes");

            migrationBuilder.DropColumn(
                name: "WasDeleted",
                table: "Aeroportos");
        }
    }
}
