using Microsoft.EntityFrameworkCore.Migrations;

namespace CSAppBE.Web.Migrations
{
    public partial class AddingFileType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_File_CertificateId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_File",
                table: "File");

            migrationBuilder.RenameTable(
                name: "File",
                newName: "Files");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Files",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Files_CertificateId",
                table: "AspNetUsers",
                column: "CertificateId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Files_CertificateId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "File");

            migrationBuilder.AddPrimaryKey(
                name: "PK_File",
                table: "File",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_File_CertificateId",
                table: "AspNetUsers",
                column: "CertificateId",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
