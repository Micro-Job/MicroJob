using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedSummaryColumnsAndResumeLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSoft",
                table: "Skills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Resumes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResumeLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkType = table.Column<byte>(type: "tinyint", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeLinks_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumeLinks_ResumeId",
                table: "ResumeLinks",
                column: "ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumeLinks");

            migrationBuilder.DropColumn(
                name: "IsSoft",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Resumes");
        }
    }
}
