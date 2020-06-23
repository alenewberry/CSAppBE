using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSAppBE.Web.Migrations
{
    public partial class AddingFileForCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AddedDate = table.Column<DateTime>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CertificateId",
                table: "AspNetUsers",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_File_CertificateId",
                table: "AspNetUsers",
                column: "CertificateId",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_File_CertificateId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CertificateId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "AspNetUsers");
        }
    }
}
