using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Likes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Postagem");

            migrationBuilder.DropColumn(
                name: "AlvoNome",
                table: "Mensagem");

            migrationBuilder.CreateTable(
                name: "ComentarioDeComentario",
                columns: table => new
                {
                    ComentarioFonteId = table.Column<int>(type: "int", nullable: false),
                    ComentarioFilhoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioDeComentario", x => new { x.ComentarioFonteId, x.ComentarioFilhoId });
                    table.ForeignKey(
                        name: "FK_ComentarioDeComentario_Comentario_ComentarioFilhoId",
                        column: x => x.ComentarioFilhoId,
                        principalTable: "Comentario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentarioDeComentario_Comentario_ComentarioFonteId",
                        column: x => x.ComentarioFonteId,
                        principalTable: "Comentario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.PostagemId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Postagem_PostagemId",
                        column: x => x.PostagemId,
                        principalTable: "Postagem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikesComentarios",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ComentarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikesComentarios", x => new { x.ComentarioId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_LikesComentarios_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikesComentarios_Comentario_ComentarioId",
                        column: x => x.ComentarioId,
                        principalTable: "Comentario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioDeComentario_ComentarioFilhoId",
                table: "ComentarioDeComentario",
                column: "ComentarioFilhoId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UsuarioId",
                table: "Likes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_LikesComentarios_UsuarioId",
                table: "LikesComentarios",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentarioDeComentario");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "LikesComentarios");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Postagem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AlvoNome",
                table: "Mensagem",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
