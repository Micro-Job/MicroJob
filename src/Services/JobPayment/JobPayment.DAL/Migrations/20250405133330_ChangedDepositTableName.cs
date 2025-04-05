using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDepositTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Balances_BalanceId",
                table: "Deposit");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposit_Tranzactions_TransactionId",
                table: "Deposit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deposit",
                table: "Deposit");

            migrationBuilder.RenameTable(
                name: "Deposit",
                newName: "Deposits");

            migrationBuilder.RenameIndex(
                name: "IX_Deposit_TransactionId",
                table: "Deposits",
                newName: "IX_Deposits_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposit_BalanceId",
                table: "Deposits",
                newName: "IX_Deposits_BalanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Balances_BalanceId",
                table: "Deposits",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Tranzactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Balances_BalanceId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits");

            migrationBuilder.RenameTable(
                name: "Deposits",
                newName: "Deposit");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_TransactionId",
                table: "Deposit",
                newName: "IX_Deposit_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_BalanceId",
                table: "Deposit",
                newName: "IX_Deposit_BalanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deposit",
                table: "Deposit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposit_Balances_BalanceId",
                table: "Deposit",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
