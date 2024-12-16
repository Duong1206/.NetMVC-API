using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanSach.DataAcess.Migrations
{
    public partial class AddIsDeletedToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderDetails");
        }
    }
}
