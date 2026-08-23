using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.OrderAPI.Migrations
{
    public partial class ReplaceStripePaymentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeSessionId",
                table: "OrderHeaders",
                newName: "RazorpayOrderId");

            migrationBuilder.AddColumn<string>(
                name: "RazorpayPaymentId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazorpaySignature",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RazorpayPaymentId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "RazorpaySignature",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "RazorpayOrderId",
                table: "OrderHeaders",
                newName: "StripeSessionId");
        }
    }
}
