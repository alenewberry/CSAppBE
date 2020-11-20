using Microsoft.EntityFrameworkCore.Migrations;

namespace CSAppBE.Web.Migrations
{
    public partial class AddingCommunications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cuit = table.Column<string>(nullable: true),
                    PublishedDate = table.Column<string>(nullable: true),
                    PublicSystemId = table.Column<string>(nullable: true),
                    PublicSystemDesc = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusDesc = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Attach = table.Column<bool>(nullable: false),
                    Ref1 = table.Column<string>(nullable: true),
                    Ref2 = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communications_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communications_ClientId",
                table: "Communications",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Communications");
        }
    }
}
