using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS_Gate.Migrations
{
    public partial class t1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    phone_num = table.Column<string>(nullable: false),
                    text = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    fail_count = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    sended = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
