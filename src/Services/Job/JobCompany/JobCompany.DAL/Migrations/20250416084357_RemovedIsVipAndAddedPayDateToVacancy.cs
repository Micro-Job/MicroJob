using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedIsVipAndAddedPayDateToVacancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "Vacancies");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Vacancies",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Vacancies");

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "Vacancies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
