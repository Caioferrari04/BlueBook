using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueBook.Migrations
{
    public partial class Friendship_new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amizade_AspNetUsers_AlvoId",
                table: "Amizade");

            migrationBuilder.DropForeignKey(
                name: "FK_Amizade_AspNetUsers_OrigemId",
                table: "Amizade");

            migrationBuilder.AddForeignKey(
                name: "FK_Amizade_AspNetUsers_AlvoId",
                table: "Amizade",
                column: "AlvoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Amizade_AspNetUsers_OrigemId",
                table: "Amizade",
                column: "OrigemId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amizade_AspNetUsers_AlvoId",
                table: "Amizade");

            migrationBuilder.DropForeignKey(
                name: "FK_Amizade_AspNetUsers_OrigemId",
                table: "Amizade");

            migrationBuilder.AddForeignKey(
                name: "FK_Amizade_AspNetUsers_AlvoId",
                table: "Amizade",
                column: "AlvoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Amizade_AspNetUsers_OrigemId",
                table: "Amizade",
                column: "OrigemId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
