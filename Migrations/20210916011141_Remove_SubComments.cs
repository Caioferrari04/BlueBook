using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Remove_SubComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentarioDeComentario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioDeComentario_ComentarioFilhoId",
                table: "ComentarioDeComentario",
                column: "ComentarioFilhoId");
        }
    }
}
