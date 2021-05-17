using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBContext.Migrations
{
    public partial class add_luckyWheel_DilyBonus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyBonusId",
                table: "Prize",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LuckyWheelId",
                table: "Prize",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyBonus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartsCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBonus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LuckyWheel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    PartsCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.0000000')"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuckyWheel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LuckyWheel_Group",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Question",
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
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_AppTask_AppTaskId",
                        column: x => x.AppTaskId,
                        principalTable: "AppTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScriptAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Script = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    IsRightOption = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOption_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    QuestionOptionId = table.Column<int>(type: "int", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRightAnswer = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAnswer_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAnswer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAnswer_QuestionOption_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "QuestionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prize_DailyBonusId",
                table: "Prize",
                column: "DailyBonusId");

            migrationBuilder.CreateIndex(
                name: "IX_Prize_LuckyWheelId",
                table: "Prize",
                column: "LuckyWheelId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAnswer_CustomerId",
                table: "CustomerAnswer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAnswer_QuestionId",
                table: "CustomerAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAnswer_QuestionOptionId",
                table: "CustomerAnswer",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_LuckyWheel_GroupId",
                table: "LuckyWheel",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_AppTaskId",
                table: "Question",
                column: "AppTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOption_QuestionId",
                table: "QuestionOption",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prize_DailyBonus_DailyBonusId",
                table: "Prize",
                column: "DailyBonusId",
                principalTable: "DailyBonus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prize_LuckyWheel_LuckyWheelId",
                table: "Prize",
                column: "LuckyWheelId",
                principalTable: "LuckyWheel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prize_DailyBonus_DailyBonusId",
                table: "Prize");

            migrationBuilder.DropForeignKey(
                name: "FK_Prize_LuckyWheel_LuckyWheelId",
                table: "Prize");

            migrationBuilder.DropTable(
                name: "CustomerAnswer");

            migrationBuilder.DropTable(
                name: "DailyBonus");

            migrationBuilder.DropTable(
                name: "LuckyWheel");

            migrationBuilder.DropTable(
                name: "QuestionOption");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Prize_DailyBonusId",
                table: "Prize");

            migrationBuilder.DropIndex(
                name: "IX_Prize_LuckyWheelId",
                table: "Prize");

            migrationBuilder.DropColumn(
                name: "DailyBonusId",
                table: "Prize");

            migrationBuilder.DropColumn(
                name: "LuckyWheelId",
                table: "Prize");
        }
    }
}
