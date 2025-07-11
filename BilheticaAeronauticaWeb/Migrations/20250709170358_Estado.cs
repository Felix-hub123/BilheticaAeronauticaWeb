using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class Estado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bilhetes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_UserId",
                table: "Bilhetes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bilhetes_AspNetUsers_UserId",
                table: "Bilhetes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bilhetes_AspNetUsers_UserId",
                table: "Bilhetes");

            migrationBuilder.DropIndex(
                name: "IX_Bilhetes_UserId",
                table: "Bilhetes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bilhetes");
        }
    }
}
