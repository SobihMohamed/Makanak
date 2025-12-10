using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Makanak.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DisputeandDisputesImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Booking_BookingId",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Users_ComplaintId",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_DisputeImages_Dispute_DisputeId",
                table: "DisputeImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dispute",
                table: "Dispute");

            migrationBuilder.RenameTable(
                name: "Dispute",
                newName: "Disputes");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_ComplaintId",
                table: "Disputes",
                newName: "IX_Disputes_ComplaintId");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_BookingId",
                table: "Disputes",
                newName: "IX_Disputes_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Disputes",
                table: "Disputes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DisputeImages_Disputes_DisputeId",
                table: "DisputeImages",
                column: "DisputeId",
                principalTable: "Disputes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Booking_BookingId",
                table: "Disputes",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_ComplaintId",
                table: "Disputes",
                column: "ComplaintId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisputeImages_Disputes_DisputeId",
                table: "DisputeImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Booking_BookingId",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_ComplaintId",
                table: "Disputes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Disputes",
                table: "Disputes");

            migrationBuilder.RenameTable(
                name: "Disputes",
                newName: "Dispute");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_ComplaintId",
                table: "Dispute",
                newName: "IX_Dispute_ComplaintId");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_BookingId",
                table: "Dispute",
                newName: "IX_Dispute_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dispute",
                table: "Dispute",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Booking_BookingId",
                table: "Dispute",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Users_ComplaintId",
                table: "Dispute",
                column: "ComplaintId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DisputeImages_Dispute_DisputeId",
                table: "DisputeImages",
                column: "DisputeId",
                principalTable: "Dispute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
