using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VirtualGlassesProvider.Migrations
{
    /// <inheritdoc />
    public partial class Glasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Glasses",
                columns: table => new
                {
                    glassesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    glassesBrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Glasses", x => x.glassesID);
                });

            migrationBuilder.InsertData(
                table: "Glasses",
                columns: new[] { "glassesID", "Description", "Price", "Style", "color", "glassesBrandName" },
                values: new object[,]
                {
                    { 1, "Black Colour Sqaured shaped Rayban Sunglasses", 10.99m, "Square", "Black", "Rayban" },
                    { 2, "Blue Colour Circular shaped Rayban Sunglasses", 10.99m, "Circular", "Blue", "Rayban" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Glasses");
        }
    }
}
