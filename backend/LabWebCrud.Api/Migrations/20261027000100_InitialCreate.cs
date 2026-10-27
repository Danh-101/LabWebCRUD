using LabWebCrud.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabWebCrud.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261027000100_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                Price = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Products", x => x.Id));

        migrationBuilder.InsertData(
            table: "Products",
            columns: new[] { "Id", "Name", "Description", "Price", "Quantity" },
            values: new object[,]
            {
                { 1, "Ban phim co", "Ban phim co co den RGB", 690000, 12 },
                { 2, "Chuot khong day", "Chuot Bluetooth va USB", 350000, 20 },
                { 3, "Tai nghe", "Tai nghe chup tai", 450000, 8 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Products");
    }
}
