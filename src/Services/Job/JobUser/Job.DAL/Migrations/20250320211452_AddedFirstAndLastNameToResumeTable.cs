using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedFirstAndLastNameToResumeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Resumes",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Resumes",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Resumes");
        }
    }
}
