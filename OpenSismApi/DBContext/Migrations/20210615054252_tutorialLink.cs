using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class tutorialLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TutorialLink",
                table: "AppTask",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TutorialLink",
                table: "AppTask");
        }
    }
}
