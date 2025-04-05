using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Tranzactions_TransactionId",
                table: "Deposit");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Packets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompany",
                table: "Balances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Tranzactions_TransactionId",
                table: "Deposit",
                column: "TransactionId",
                principalTable: "Tranzactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Tranzactions_TransactionId",
                table: "Deposit");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Packets");

            migrationBuilder.DropColumn(
                name: "IsCompany",
                table: "Balances");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Tranzactions_TransactionId",
                table: "Deposit",
                column: "TransactionId",
                principalTable: "Tranzactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
