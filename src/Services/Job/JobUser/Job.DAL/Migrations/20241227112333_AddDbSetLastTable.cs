using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSetLastTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_Users_UserId",
                table: "UserAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExam_Users_UserId",
                table: "UserExam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserExam",
                table: "UserExam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswer",
                table: "UserAnswer");

            migrationBuilder.RenameTable(
                name: "UserExam",
                newName: "UserExams");

            migrationBuilder.RenameTable(
                name: "UserAnswer",
                newName: "UserAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_UserExam_UserId",
                table: "UserExams",
                newName: "IX_UserExams_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswer_UserId",
                table: "UserAnswers",
                newName: "IX_UserAnswers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserExams",
                table: "UserExams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Users_UserId",
                table: "UserAnswers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExams_Users_UserId",
                table: "UserExams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Users_UserId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExams_Users_UserId",
                table: "UserExams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserExams",
                table: "UserExams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers");

            migrationBuilder.RenameTable(
                name: "UserExams",
                newName: "UserExam");

            migrationBuilder.RenameTable(
                name: "UserAnswers",
                newName: "UserAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_UserExams_UserId",
                table: "UserExam",
                newName: "IX_UserExam_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswers_UserId",
                table: "UserAnswer",
                newName: "IX_UserAnswer_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserExam",
                table: "UserExam",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswer",
                table: "UserAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_Users_UserId",
                table: "UserAnswer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExam_Users_UserId",
                table: "UserExam",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
