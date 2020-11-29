using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class AddCuponUserKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cupons_CreatorId",
                table: "Cupons",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cupons_Guests_CreatorId",
                table: "Cupons",
                column: "CreatorId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cupons_Guests_CreatorId",
                table: "Cupons");

            migrationBuilder.DropIndex(
                name: "IX_Cupons_CreatorId",
                table: "Cupons");
        }
    }
}
