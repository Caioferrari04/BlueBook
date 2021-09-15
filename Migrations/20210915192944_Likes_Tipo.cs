using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Likes_Tipo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoLike",
                table: "LikesComentarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoLike",
                table: "Likes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoLike",
                table: "LikesComentarios");

            migrationBuilder.DropColumn(
                name: "TipoLike",
                table: "Likes");
        }
    }
}
