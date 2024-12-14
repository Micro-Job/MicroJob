using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class jobstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "JobStatus",
                table: "Users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobStatus",
                table: "Users");
        }
    }
}
