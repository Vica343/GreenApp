using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class UserCupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Cupons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserCupon",
                columns: table => new
                {
                    CuponId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCupon", x => new { x.UserId, x.CuponId });
                    table.ForeignKey(
                        name: "FK_UserCupon_Cupons_CuponId",
                        column: x => x.CuponId,
                        principalTable: "Cupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCupon_Guests_UserId",
                        column: x => x.UserId,
                        principalTable: "Guests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCupon_CuponId",
                table: "UserCupon",
                column: "CuponId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCupon");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Cupons");
        }
    }
}
