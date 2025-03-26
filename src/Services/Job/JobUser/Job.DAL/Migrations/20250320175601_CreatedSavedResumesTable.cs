using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreatedSavedResumesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes");

            migrationBuilder.CreateTable(
                name: "SavedResumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaveDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedResumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedResumes_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedResumes_Users_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_CompanyUserId",
                table: "SavedResumes",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_ResumeId",
                table: "SavedResumes",
                column: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes");

            migrationBuilder.DropTable(
                name: "SavedResumes");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Users_UserId",
                table: "Resumes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
