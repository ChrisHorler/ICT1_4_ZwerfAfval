using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlApi.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTrashTypeIdWithString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detection_TrashType_trashTypeId",
                table: "Detection");

            migrationBuilder.DropTable(
                name: "TrashType");

            migrationBuilder.DropIndex(
                name: "IX_Detection_trashTypeId",
                table: "Detection");

            migrationBuilder.DropColumn(
                name: "trashTypeId",
                table: "Detection");

            migrationBuilder.AddColumn<string>(
                name: "trashType",
                table: "Detection",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trashType",
                table: "Detection");

            migrationBuilder.AddColumn<int>(
                name: "trashTypeId",
                table: "Detection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TrashType",
                columns: table => new
                {
                    trashTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrashType", x => x.trashTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Detection_trashTypeId",
                table: "Detection",
                column: "trashTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detection_TrashType_trashTypeId",
                table: "Detection",
                column: "trashTypeId",
                principalTable: "TrashType",
                principalColumn: "trashTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
