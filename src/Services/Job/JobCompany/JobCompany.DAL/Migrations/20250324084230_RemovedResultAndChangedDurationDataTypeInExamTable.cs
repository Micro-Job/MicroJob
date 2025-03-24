using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedResultAndChangedDurationDataTypeInExamTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "Exams");

            migrationBuilder.AlterColumn<short>(
                name: "Duration",
                table: "Exams",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Duration",
                table: "Exams",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
