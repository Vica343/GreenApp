using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class AddChallengesToCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Challenges_CreatorId",
                table: "Challenges",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Guests_CreatorId",
                table: "Challenges",
                column: "CreatorId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Guests_CreatorId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_CreatorId",
                table: "Challenges");
        }
    }
}
