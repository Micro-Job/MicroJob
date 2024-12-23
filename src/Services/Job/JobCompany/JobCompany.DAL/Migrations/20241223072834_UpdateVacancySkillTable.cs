using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVacancySkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacancySkill_Skills_SkillId",
                table: "VacancySkill");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancySkill_Vacancies_VacancyId",
                table: "VacancySkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacancySkill",
                table: "VacancySkill");

            migrationBuilder.DropIndex(
                name: "IX_VacancySkill_VacancyId",
                table: "VacancySkill");

            migrationBuilder.RenameTable(
                name: "VacancySkill",
                newName: "VacancySkills");

            migrationBuilder.RenameIndex(
                name: "IX_VacancySkill_SkillId",
                table: "VacancySkills",
                newName: "IX_VacancySkills_SkillId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacancySkills",
                table: "VacancySkills",
                columns: new[] { "VacancyId", "SkillId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VacancySkills_Skills_SkillId",
                table: "VacancySkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancySkills_Vacancies_VacancyId",
                table: "VacancySkills",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacancySkills_Skills_SkillId",
                table: "VacancySkills");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancySkills_Vacancies_VacancyId",
                table: "VacancySkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacancySkills",
                table: "VacancySkills");

            migrationBuilder.RenameTable(
                name: "VacancySkills",
                newName: "VacancySkill");

            migrationBuilder.RenameIndex(
                name: "IX_VacancySkills_SkillId",
                table: "VacancySkill",
                newName: "IX_VacancySkill_SkillId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacancySkill",
                table: "VacancySkill",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VacancySkill_VacancyId",
                table: "VacancySkill",
                column: "VacancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacancySkill_Skills_SkillId",
                table: "VacancySkill",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancySkill_Vacancies_VacancyId",
                table: "VacancySkill",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
