using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTransactionTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Tranzactions_Balances_BalanceId",
                table: "Tranzactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tranzactions",
                table: "Tranzactions");

            migrationBuilder.RenameTable(
                name: "Tranzactions",
                newName: "Transactions");

            migrationBuilder.RenameIndex(
                name: "IX_Tranzactions_BalanceId",
                table: "Transactions",
                newName: "IX_Transactions_BalanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Balances_BalanceId",
                table: "Transactions",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Transactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Balances_BalanceId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Tranzactions");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_BalanceId",
                table: "Tranzactions",
                newName: "IX_Tranzactions_BalanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tranzactions",
                table: "Tranzactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Tranzactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tranzactions_Balances_BalanceId",
                table: "Tranzactions",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
