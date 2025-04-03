using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPayment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: false),
                    BonusCoin = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PacketType = table.Column<byte>(type: "tinyint", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tranzactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformationType = table.Column<byte>(type: "tinyint", nullable: false),
                    TranzactionType = table.Column<byte>(type: "tinyint", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BalanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tranzactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tranzactions_Balances_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "Balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OldPackets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Coin = table.Column<double>(type: "float", nullable: false),
                    PacketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OldPackets_Packets_PacketId",
                        column: x => x.PacketId,
                        principalTable: "Packets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OldPackets_PacketId",
                table: "OldPackets",
                column: "PacketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tranzactions_BalanceId",
                table: "Tranzactions",
                column: "BalanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OldPackets");

            migrationBuilder.DropTable(
                name: "Tranzactions");

            migrationBuilder.DropTable(
                name: "Packets");

            migrationBuilder.DropTable(
                name: "Balances");
        }
    }
}
