using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyInvoiceWeb.Data.Migrations
{
    public partial class DescriptionToInvoiceMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Invoice");
        }
    }
}
