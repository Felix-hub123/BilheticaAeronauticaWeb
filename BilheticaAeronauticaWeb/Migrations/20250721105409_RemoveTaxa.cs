using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilheticaAeronauticaWeb.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaxa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxaAeroporto",
                table: "Voos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxaAeroporto",
                table: "Voos",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
