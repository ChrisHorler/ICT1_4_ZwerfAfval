using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlApi.Migrations
{
    /// <inheritdoc />
    public partial class migrane2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "predictedFillLevel",
                table: "Prediction");

            migrationBuilder.AlterColumn<string>(
                name: "confidence",
                table: "Prediction",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "confidence",
                table: "Prediction",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "predictedFillLevel",
                table: "Prediction",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
