using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTypeAddedNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "NotificationType",
                table: "Notifications",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Notifications");
        }
    }
}
