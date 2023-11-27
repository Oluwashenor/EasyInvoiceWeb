using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyInvoiceWeb.Data.Migrations
{
    public partial class UpdatePaymentMigrationAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payment");

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "Payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Payment");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
