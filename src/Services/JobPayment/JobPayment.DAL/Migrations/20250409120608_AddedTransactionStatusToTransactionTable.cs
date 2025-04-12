using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedTransactionStatusToTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropTable(
                name: "OldPrices");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.AddColumn<byte>(
                name: "TransactionStatus",
                table: "Tranzactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformationType = table.Column<byte>(type: "tinyint", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OldServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldCoin = table.Column<double>(type: "float", nullable: false),
                    PriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OldServices_Services_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OldServices_PriceId",
                table: "OldServices",
                column: "PriceId");

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
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits");

            migrationBuilder.DropTable(
                name: "OldServices");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "Tranzactions");

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: false),
                    InformationType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OldPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldCoin = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OldPrices_Prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OldPrices_PriceId",
                table: "OldPrices",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Tranzactions_TransactionId",
                table: "Deposits",
                column: "TransactionId",
                principalTable: "Tranzactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
