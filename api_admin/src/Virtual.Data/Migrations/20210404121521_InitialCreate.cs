using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Virtual.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_Cidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "VARCHAR(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Cidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_Jogador",
                columns: table => new
                {
                    IdJogador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Sobrenome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Posicao = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    IdUsuarioCadastro = table.Column<Guid>(type: "char(36)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Jogador", x => x.IdJogador);
                });

            migrationBuilder.CreateTable(
                name: "tb_Time",
                columns: table => new
                {
                    IdTime = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Cidade = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Bairro = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    IdUsuarioCadastro = table.Column<Guid>(type: "char(36)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Time", x => x.IdTime);
                });

            migrationBuilder.CreateTable(
                name: "tb_Jogo",
                columns: table => new
                {
                    IdJogo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdAdversario = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    Bairro = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Campo = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Hora = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    DataJogo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Gols = table.Column<int>(type: "int", nullable: false),
                    GolsAdversario = table.Column<int>(type: "int", nullable: false),
                    Resultado = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    IdUsuarioCadastro = table.Column<Guid>(type: "char(36)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Jogo", x => x.IdJogo);
                    table.ForeignKey(
                        name: "FK_tb_Jogo_tb_Cidade_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "tb_Cidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Jogo_tb_Time_IdAdversario",
                        column: x => x.IdAdversario,
                        principalTable: "tb_Time",
                        principalColumn: "IdTime",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_JogadorJogo",
                columns: table => new
                {
                    IdJogadorJogo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdJogador = table.Column<int>(type: "int", nullable: false),
                    IdJogo = table.Column<int>(type: "int", nullable: false),
                    Gols = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IdUsuarioCadastro = table.Column<Guid>(type: "char(36)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_JogadorJogo", x => x.IdJogadorJogo);
                    table.ForeignKey(
                        name: "FK_tb_JogadorJogo_tb_Jogador_IdJogador",
                        column: x => x.IdJogador,
                        principalTable: "tb_Jogador",
                        principalColumn: "IdJogador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_JogadorJogo_tb_Jogo_IdJogo",
                        column: x => x.IdJogo,
                        principalTable: "tb_Jogo",
                        principalColumn: "IdJogo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_JogadorJogo_IdJogador",
                table: "tb_JogadorJogo",
                column: "IdJogador");

            migrationBuilder.CreateIndex(
                name: "IX_tb_JogadorJogo_IdJogo",
                table: "tb_JogadorJogo",
                column: "IdJogo");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Jogo_IdAdversario",
                table: "tb_Jogo",
                column: "IdAdversario");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Jogo_IdCidade",
                table: "tb_Jogo",
                column: "IdCidade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_JogadorJogo");

            migrationBuilder.DropTable(
                name: "tb_Jogador");

            migrationBuilder.DropTable(
                name: "tb_Jogo");

            migrationBuilder.DropTable(
                name: "tb_Cidade");

            migrationBuilder.DropTable(
                name: "tb_Time");
        }
    }
}
