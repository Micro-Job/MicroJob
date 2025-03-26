using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class VacancyCommentAndTranslationTableCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VacancyCommentId",
                table: "Vacancies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VacancyComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VacancyCommentTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    VacancyCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyCommentTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyCommentTranslations_VacancyComments_VacancyCommentId",
                        column: x => x.VacancyCommentId,
                        principalTable: "VacancyComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_VacancyCommentId",
                table: "Vacancies",
                column: "VacancyCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyCommentTranslations_VacancyCommentId",
                table: "VacancyCommentTranslations",
                column: "VacancyCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_VacancyComments_VacancyCommentId",
                table: "Vacancies",
                column: "VacancyCommentId",
                principalTable: "VacancyComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_VacancyComments_VacancyCommentId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "VacancyCommentTranslations");

            migrationBuilder.DropTable(
                name: "VacancyComments");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_VacancyCommentId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "VacancyCommentId",
                table: "Vacancies");
        }
    }
}
