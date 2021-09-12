using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Private : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlvoId",
                table: "Mensagem",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlvoNome",
                table: "Mensagem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mensagem_AlvoId",
                table: "Mensagem",
                column: "AlvoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Mensagem_AspNetUsers_AlvoId",
                table: "Mensagem",
                column: "AlvoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensagem_AspNetUsers_AlvoId",
                table: "Mensagem");

            migrationBuilder.DropIndex(
                name: "IX_Mensagem_AlvoId",
                table: "Mensagem");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlvoId",
                table: "Mensagem");

            migrationBuilder.DropColumn(
                name: "AlvoNome",
                table: "Mensagem");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "AspNetUsers");
        }
    }
}
