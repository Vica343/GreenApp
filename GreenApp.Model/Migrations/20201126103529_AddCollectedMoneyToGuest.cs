using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class AddCollectedMoneyToGuest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollectedMoney",
                table: "Guests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedMoney",
                table: "Guests");
        }
    }
}
