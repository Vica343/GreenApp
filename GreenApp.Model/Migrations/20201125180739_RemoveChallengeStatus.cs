using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class RemoveChallengeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Challenges");

            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "Challenges",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "Challenges");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
