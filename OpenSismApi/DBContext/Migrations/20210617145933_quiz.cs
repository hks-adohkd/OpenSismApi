using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class quiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScriptAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Script = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<bool>(type: "bit", nullable: false),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    AppTaskId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_AppTask_AppTaskId",
                        column: x => x.AppTaskId,
                        principalTable: "AppTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScriptAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Script = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    IsRightOption = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizOption_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_AppTaskId",
                table: "Quiz",
                column: "AppTaskId",
                unique: true,
                filter: "([AppTaskId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOption_QuizId",
                table: "QuizOption",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizOption");

            migrationBuilder.DropTable(
                name: "Quiz");
        }
    }
}
