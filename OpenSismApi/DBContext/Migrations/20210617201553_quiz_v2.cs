using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class quiz_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quiz_AppTaskId",
                table: "Quiz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "Quiz",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "('0001-01-01T00:00:00.0000000')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Quiz",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "('0001-01-01T00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_AppTaskId",
                table: "Quiz",
                column: "AppTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quiz_AppTaskId",
                table: "Quiz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "Quiz",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "('0001-01-01T00:00:00.0000000')",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Quiz",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "('0001-01-01T00:00:00.0000000')",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_AppTaskId",
                table: "Quiz",
                column: "AppTaskId",
                unique: true,
                filter: "([AppTaskId] IS NOT NULL)");
        }
    }
}
