using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanSach.DataAcess.Migrations
{
    public partial class updateCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Coupons");
        }
    }
}
