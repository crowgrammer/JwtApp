using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jwt.API.Migrations
{
    public partial class ordersmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Creatorname",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creatorname",
                table: "Orders");
        }
    }
}
