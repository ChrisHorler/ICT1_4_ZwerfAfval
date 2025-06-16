using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlApi.Migrations
{
    /// <inheritdoc />
    public partial class migrane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_POI_POICategory_categoryId",
                table: "POI");

            migrationBuilder.DropTable(
                name: "POICategory");

            migrationBuilder.DropIndex(
                name: "IX_POI_categoryId",
                table: "POI");

            migrationBuilder.DropColumn(
                name: "categoryId",
                table: "POI");

            migrationBuilder.RenameColumn(
                name: "elementType",
                table: "POI",
                newName: "category");

            migrationBuilder.AlterColumn<long>(
                name: "osmId",
                table: "POI",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "longitude",
                table: "POI",
                type: "double",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "latitude",
                table: "POI",
                type: "double",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<float>(
                name: "detectionRadiusM",
                table: "DetectionPOI",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "detectionRadiusM",
                table: "DetectionPOI");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "POI",
                newName: "elementType");

            migrationBuilder.AlterColumn<int>(
                name: "osmId",
                table: "POI",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<float>(
                name: "longitude",
                table: "POI",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<float>(
                name: "latitude",
                table: "POI",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<int>(
                name: "categoryId",
                table: "POI",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "POICategory",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POICategory", x => x.categoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_POI_categoryId",
                table: "POI",
                column: "categoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_POI_POICategory_categoryId",
                table: "POI",
                column: "categoryId",
                principalTable: "POICategory",
                principalColumn: "categoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
