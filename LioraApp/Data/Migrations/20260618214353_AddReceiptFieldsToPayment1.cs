using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LioraApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptFieldsToPayment1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptImageUrl",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptPublicId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptImageUrl",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReceiptPublicId",
                table: "Payments");
        }
    }
}
