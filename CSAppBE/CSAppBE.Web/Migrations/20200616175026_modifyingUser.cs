using Microsoft.EntityFrameworkCore.Migrations;

namespace CSAppBE.Web.Migrations
{
    public partial class modifyingUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_userId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Clients",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_userId",
                table: "Clients",
                newName: "IX_Clients_UserId");

            migrationBuilder.AddColumn<string>(
                name: "CUIT",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CUIT",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Clients",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                newName: "IX_Clients_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_userId",
                table: "Clients",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
