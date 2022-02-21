using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web_shop.DataAccess.Migrations
{
    public partial class ChangeModelsAndAddNewModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetails_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetails_Products_ProductId",
                table: "InquiryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryHeaders_AspNetUsers_ApplicationUserId",
                table: "InquiryHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryHeaders",
                table: "InquiryHeaders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryDetails",
                table: "InquiryDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "InquiryHeaders",
                newName: "InquiryHeader");

            migrationBuilder.RenameTable(
                name: "InquiryDetails",
                newName: "InquiryDetail");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "ShortDescriptionription",
                table: "Product",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "Desctiption",
                table: "Product",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Product",
                newName: "IX_Product_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryHeaders_ApplicationUserId",
                table: "InquiryHeader",
                newName: "IX_InquiryHeader_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetails_ProductId",
                table: "InquiryDetail",
                newName: "IX_InquiryDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetails_InquiryHeaderId",
                table: "InquiryDetail",
                newName: "IX_InquiryDetail_InquiryHeaderId");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Product",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationTypeId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryHeader",
                table: "InquiryHeader",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryDetail",
                table: "InquiryDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApplicationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHeader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalOrderTotal = table.Column<double>(type: "float", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHeader_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Sqft = table.Column<int>(type: "int", nullable: false),
                    PricePerSqFt = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_OrderHeader_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderHeaderId",
                table: "OrderDetail",
                column: "OrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_CreatedByUserId",
                table: "OrderHeader",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_InquiryHeader_InquiryHeaderId",
                table: "InquiryDetail",
                column: "InquiryHeaderId",
                principalTable: "InquiryHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryHeader_AspNetUsers_ApplicationUserId",
                table: "InquiryHeader",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId",
                principalTable: "ApplicationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_InquiryHeader_InquiryHeaderId",
                table: "InquiryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryHeader_AspNetUsers_ApplicationUserId",
                table: "InquiryHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ApplicationType");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "OrderHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ApplicationTypeId",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryHeader",
                table: "InquiryHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryDetail",
                table: "InquiryDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeId",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "InquiryHeader",
                newName: "InquiryHeaders");

            migrationBuilder.RenameTable(
                name: "InquiryDetail",
                newName: "InquiryDetails");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Products",
                newName: "ShortDescriptionription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "Desctiption");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryHeader_ApplicationUserId",
                table: "InquiryHeaders",
                newName: "IX_InquiryHeaders_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetails",
                newName: "IX_InquiryDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetail_InquiryHeaderId",
                table: "InquiryDetails",
                newName: "IX_InquiryDetails_InquiryHeaderId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryHeaders",
                table: "InquiryHeaders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryDetails",
                table: "InquiryDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetails_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetails",
                column: "InquiryHeaderId",
                principalTable: "InquiryHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetails_Products_ProductId",
                table: "InquiryDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryHeaders_AspNetUsers_ApplicationUserId",
                table: "InquiryHeaders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
