using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Friendship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Amizade",
                columns: table => new
                {
                    OrigemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlvoId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amizade", x => new { x.OrigemId, x.AlvoId });
                    table.ForeignKey(
                        name: "FK_Amizade_AspNetUsers_AlvoId",
                        column: x => x.AlvoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amizade_AspNetUsers_OrigemId",
                        column: x => x.OrigemId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amizade_AlvoId",
                table: "Amizade",
                column: "AlvoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amizade");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UsuarioId",
                table: "AspNetUsers",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UsuarioId",
                table: "AspNetUsers",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
