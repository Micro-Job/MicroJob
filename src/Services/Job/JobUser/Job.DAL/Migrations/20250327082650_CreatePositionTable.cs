using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreatePositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Resumes");

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ParentPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Positions_ParentPositionId",
                        column: x => x.ParentPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_PositionId",
                table: "Resumes",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ParentPositionId",
                table: "Positions",
                column: "ParentPositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Positions_PositionId",
                table: "Resumes",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Positions_PositionId",
                table: "Resumes");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_PositionId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Resumes");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}