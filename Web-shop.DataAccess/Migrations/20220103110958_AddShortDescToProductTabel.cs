using Microsoft.EntityFrameworkCore.Migrations;

namespace Web_shop.DataAccess.Migrations
{
    public partial class AddShortDescriptionToProductTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescriptionription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescriptionription",
                table: "Products");
        }
    }
}
