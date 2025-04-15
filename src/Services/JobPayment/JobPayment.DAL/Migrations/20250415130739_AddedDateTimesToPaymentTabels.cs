using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateTimesToPaymentTabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Services",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Packets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Packets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "Packets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OldServices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OldPackets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Packets");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Packets");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "Packets");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OldServices");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OldPackets");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
