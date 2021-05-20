using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class added_mail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecieverEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mail");
        }
    }
}
