using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bunnymail.Migrations
{
    public partial class Bunnymail_M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTemplate",
                columns: table => new
                {
                    Event = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    TemplateID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FromName = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    FromAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTemplate", x => x.Event);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTemplate");
        }
    }
}
