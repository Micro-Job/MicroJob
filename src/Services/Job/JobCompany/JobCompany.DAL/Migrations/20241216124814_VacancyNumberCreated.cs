using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class VacancyNumberCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyNumbers_Vacancies_VacancyId",
                table: "CompanyNumbers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyNumbers_VacancyId",
                table: "CompanyNumbers");

            migrationBuilder.DropColumn(
                name: "VacancyId",
                table: "CompanyNumbers");

            migrationBuilder.CreateTable(
                name: "VacancyNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    VacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyNumbers_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyNumbers_VacancyId",
                table: "VacancyNumbers",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyNumbers");

            migrationBuilder.AddColumn<Guid>(
                name: "VacancyId",
                table: "CompanyNumbers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNumbers_VacancyId",
                table: "CompanyNumbers",
                column: "VacancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyNumbers_Vacancies_VacancyId",
                table: "CompanyNumbers",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
