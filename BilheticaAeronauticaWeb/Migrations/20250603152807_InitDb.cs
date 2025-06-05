using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BilheticaAeronauticaWeb.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aeroportos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IATA = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aeroportos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Avioes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LugaresEconomica = table.Column<int>(type: "int", nullable: false),
                    LugaresExecutiva = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avioes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passageiros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentoIdentificacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passageiros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fila = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Classe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    AeronaveId = table.Column<int>(type: "int", nullable: false),
                    AviaoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lugares_Avioes_AviaoId",
                        column: x => x.AviaoId,
                        principalTable: "Avioes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                 name: "Voos",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                     .Annotation("SqlServer:Identity", "1, 1"),
                     NumeroVoo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                     DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                     Ativo = table.Column<bool>(type: "bit", nullable: false),
                     OrigemId = table.Column<int>(type: "int", nullable: false),
                     DestinoId = table.Column<int>(type: "int", nullable: false),
                     AeronaveId = table.Column<int>(type: "int", nullable: false),
                     AviaoId = table.Column<int>(type: "int", nullable: true)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Voos", x => x.Id);
                     table.ForeignKey(
                          name: "FK_Voos_Aeroportos_DestinoId",
                          column: x => x.DestinoId,
                          principalTable: "Aeroportos",
                          principalColumn: "Id",
                          onDelete: ReferentialAction.Restrict); 
                     table.ForeignKey(
                           name: "FK_Voos_Aeroportos_OrigemId",
                           column: x => x.OrigemId,
                           principalTable: "Aeroportos",
                           principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict); 
                     table.ForeignKey(
                       name: "FK_Voos_Avioes_AviaoId",
                       column: x => x.AviaoId,
                       principalTable: "Avioes",
                       principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
                 });

            migrationBuilder.CreateTable(
                name: "Bilhetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarifa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Classe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PodeAlterar = table.Column<bool>(type: "bit", nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VooId = table.Column<int>(type: "int", nullable: false),
                    LugarId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PassageiroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bilhetes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Lugares_LugarId",
                        column: x => x.LugarId,
                        principalTable: "Lugares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Passageiros_PassageiroId",
                        column: x => x.PassageiroId,
                        principalTable: "Passageiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bilhetes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Voos_VooId",
                        column: x => x.VooId,
                        principalTable: "Voos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_LugarId",
                table: "Bilhetes",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_PassageiroId",
                table: "Bilhetes",
                column: "PassageiroId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_UserId",
                table: "Bilhetes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_VooId",
                table: "Bilhetes",
                column: "VooId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_AviaoId",
                table: "Lugares",
                column: "AviaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Voos_AviaoId",
                table: "Voos",
                column: "AviaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Voos_DestinoId",
                table: "Voos",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Voos_OrigemId",
                table: "Voos",
                column: "OrigemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bilhetes");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropTable(
                name: "Passageiros");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Voos");

            migrationBuilder.DropTable(
                name: "Aeroportos");

            migrationBuilder.DropTable(
                name: "Avioes");
        }
    }
}
