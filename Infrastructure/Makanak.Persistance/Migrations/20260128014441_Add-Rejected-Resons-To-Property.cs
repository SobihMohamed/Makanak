using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Makanak.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedResonsToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "Properties");
        }
    }
}
