using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Makanak.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addnewcoulmnfordeadlineforpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDeadline",
                table: "Booking",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDeadline",
                table: "Booking");
        }
    }
}
