using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyInvoiceWeb.Data.Migrations
{
    public partial class ClientMigrationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Client");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
