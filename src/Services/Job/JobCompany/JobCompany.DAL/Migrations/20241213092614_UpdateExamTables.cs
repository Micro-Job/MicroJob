using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExamTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_VacancyTests_VacancyTestId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "VacancyTests");

            migrationBuilder.RenameColumn(
                name: "VacancyTestId",
                table: "Vacancies",
                newName: "ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_Vacancies_VacancyTestId",
                table: "Vacancies",
                newName: "IX_Vacancies_ExamId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompany",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTemplate = table.Column<bool>(type: "bit", nullable: false),
                    IntroDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VacancySkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancySkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancySkill_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancySkill_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CompanyId",
                table: "Exams",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancySkill_SkillId",
                table: "VacancySkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancySkill_VacancyId",
                table: "VacancySkill",
                column: "VacancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Exams_ExamId",
                table: "Vacancies",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Exams_ExamId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "VacancySkill");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropColumn(
                name: "IsCompany",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "Vacancies",
                newName: "VacancyTestId");

            migrationBuilder.RenameIndex(
                name: "IX_Vacancies_ExamId",
                table: "Vacancies",
                newName: "IX_Vacancies_VacancyTestId");

            migrationBuilder.CreateTable(
                name: "VacancyTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyTests", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_VacancyTests_VacancyTestId",
                table: "Vacancies",
                column: "VacancyTestId",
                principalTable: "VacancyTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
