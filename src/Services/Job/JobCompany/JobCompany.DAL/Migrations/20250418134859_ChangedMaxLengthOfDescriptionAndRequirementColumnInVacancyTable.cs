using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMaxLengthOfDescriptionAndRequirementColumnInVacancyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                table: "Vacancies",
                type: "nvarchar(max)",
                maxLength: 8192,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vacancies",
                type: "nvarchar(max)",
                maxLength: 8192,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                table: "Vacancies",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8192);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vacancies",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8192);
        }
    }
}
