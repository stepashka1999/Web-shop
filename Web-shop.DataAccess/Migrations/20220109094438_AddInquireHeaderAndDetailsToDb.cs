using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web_shop.DataAccess.Migrations
{
    public partial class AddInquireHeaderAndDetailsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InquieryHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InquireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquieryHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquieryHeaders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InquieryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquieryHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquieryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquieryDetails_InquieryHeaders_InquieryHeaderId",
                        column: x => x.InquieryHeaderId,
                        principalTable: "InquieryHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquieryDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InquieryDetails_InquieryHeaderId",
                table: "InquieryDetails",
                column: "InquieryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InquieryDetails_ProductId",
                table: "InquieryDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InquieryHeaders_ApplicationUserId",
                table: "InquieryHeaders",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquieryDetails");

            migrationBuilder.DropTable(
                name: "InquieryHeaders");
        }
    }
}
