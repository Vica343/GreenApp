using Microsoft.EntityFrameworkCore.Migrations;

namespace GreenApp.Model.Migrations
{
    public partial class ChangeCupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCupon_Cupons_CuponId",
                table: "UserCupon");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCupon_Guests_UserId",
                table: "UserCupon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCupon",
                table: "UserCupon");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Cupons");

            migrationBuilder.RenameTable(
                name: "UserCupon",
                newName: "UserCupons");

            migrationBuilder.RenameIndex(
                name: "IX_UserCupon_CuponId",
                table: "UserCupons",
                newName: "IX_UserCupons_CuponId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCupons",
                table: "UserCupons",
                columns: new[] { "UserId", "CuponId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserCupons_Cupons_CuponId",
                table: "UserCupons",
                column: "CuponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCupons_Guests_UserId",
                table: "UserCupons",
                column: "UserId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCupons_Cupons_CuponId",
                table: "UserCupons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCupons_Guests_UserId",
                table: "UserCupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCupons",
                table: "UserCupons");

            migrationBuilder.RenameTable(
                name: "UserCupons",
                newName: "UserCupon");

            migrationBuilder.RenameIndex(
                name: "IX_UserCupons_CuponId",
                table: "UserCupon",
                newName: "IX_UserCupon_CuponId");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Cupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCupon",
                table: "UserCupon",
                columns: new[] { "UserId", "CuponId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserCupon_Cupons_CuponId",
                table: "UserCupon",
                column: "CuponId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCupon_Guests_UserId",
                table: "UserCupon",
                column: "UserId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
