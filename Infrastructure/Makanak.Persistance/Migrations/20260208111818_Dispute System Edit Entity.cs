using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Makanak.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DisputeSystemEditEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_ComplaintId",
                table: "Disputes");

            migrationBuilder.RenameColumn(
                name: "ComplaintId",
                table: "Disputes",
                newName: "ComplainantId");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_ComplaintId",
                table: "Disputes",
                newName: "IX_Disputes_ComplainantId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Disputes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckInReminderSent",
                table: "Booking",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentWarningSent",
                table: "Booking",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_ComplainantId",
                table: "Disputes",
                column: "ComplainantId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_ComplainantId",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "IsCheckInReminderSent",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "IsPaymentWarningSent",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "ComplainantId",
                table: "Disputes",
                newName: "ComplaintId");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_ComplainantId",
                table: "Disputes",
                newName: "IX_Disputes_ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_ComplaintId",
                table: "Disputes",
                column: "ComplaintId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
