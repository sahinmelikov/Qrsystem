using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrSystem.Migrations
{
    public partial class product_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tables_TablesId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TablesId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TablesId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductRestourantTables",
                columns: table => new
                {
                    ProductsId = table.Column<int>(type: "int", nullable: false),
                    TablesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRestourantTables", x => new { x.ProductsId, x.TablesId });
                    table.ForeignKey(
                        name: "FK_ProductRestourantTables_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductRestourantTables_Tables_TablesId",
                        column: x => x.TablesId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRestourantTables_TablesId",
                table: "ProductRestourantTables",
                column: "TablesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRestourantTables");

            migrationBuilder.AddColumn<int>(
                name: "TablesId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TablesId",
                table: "Products",
                column: "TablesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tables_TablesId",
                table: "Products",
                column: "TablesId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
