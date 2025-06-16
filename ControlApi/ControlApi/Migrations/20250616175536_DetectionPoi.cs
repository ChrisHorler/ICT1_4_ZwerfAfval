using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlApi.Migrations
{
    /// <inheritdoc />
    public partial class DetectionPoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "distanceM",
                table: "DetectionPOI");

            migrationBuilder.DropColumn(
                name: "isNearest",
                table: "DetectionPOI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "distanceM",
                table: "DetectionPOI",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "isNearest",
                table: "DetectionPOI",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
