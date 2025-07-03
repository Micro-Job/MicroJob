using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSavedResumeLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedResumes_Users_CompanyUserId",
                table: "SavedResumes");

            migrationBuilder.DropIndex(
                name: "IX_SavedResumes_CompanyUserId",
                table: "SavedResumes");

            migrationBuilder.DropIndex(
                name: "IX_SavedResumes_ResumeId",
                table: "SavedResumes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaveDate",
                table: "SavedResumes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SavedResumes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_ResumeId_CompanyUserId",
                table: "SavedResumes",
                columns: new[] { "ResumeId", "CompanyUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_UserId",
                table: "SavedResumes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedResumes_Users_UserId",
                table: "SavedResumes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedResumes_Users_UserId",
                table: "SavedResumes");

            migrationBuilder.DropIndex(
                name: "IX_SavedResumes_ResumeId_CompanyUserId",
                table: "SavedResumes");

            migrationBuilder.DropIndex(
                name: "IX_SavedResumes_UserId",
                table: "SavedResumes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SavedResumes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaveDate",
                table: "SavedResumes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_CompanyUserId",
                table: "SavedResumes",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedResumes_ResumeId",
                table: "SavedResumes",
                column: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedResumes_Users_CompanyUserId",
                table: "SavedResumes",
                column: "CompanyUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
