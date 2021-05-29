using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class DailyBonusPrmium : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "DailyBonus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "DailyBonus");
        }
    }
}
