using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    passwordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "POI",
                columns: table => new
                {
                    POIID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    osmId = table.Column<int>(type: "int", nullable: false),
                    elementType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<float>(type: "float", nullable: false),
                    longitude = table.Column<float>(type: "float", nullable: false),
                    source = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    retrievedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POI", x => x.POIID);
                    table.ForeignKey(
                        name: "FK_POI_POICategory_categoryId",
                        column: x => x.categoryId,
                        principalTable: "POICategory",
                        principalColumn: "categoryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Detection",
                columns: table => new
                {
                    detectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    trashTypeId = table.Column<int>(type: "int", nullable: false),
                    timeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    confidence = table.Column<float>(type: "float", nullable: false),
                    latitude = table.Column<float>(type: "float", nullable: false),
                    longitude = table.Column<float>(type: "float", nullable: false),
                    feelsLikeTempC = table.Column<float>(type: "float", nullable: true),
                    actualTempC = table.Column<float>(type: "float", nullable: true),
                    windForceBft = table.Column<float>(type: "float", nullable: true),
                    windDirection = table.Column<float>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detection", x => x.detectionId);
                    table.ForeignKey(
                        name: "FK_Detection_TrashType_trashTypeId",
                        column: x => x.trashTypeId,
                        principalTable: "TrashType",
                        principalColumn: "trashTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetectionPOI",
                columns: table => new
                {
                    detectionId = table.Column<int>(type: "int", nullable: false),
                    POIID = table.Column<int>(type: "int", nullable: false),
                    distanceM = table.Column<float>(type: "float", nullable: false),
                    isNearest = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectionPOI", x => new { x.detectionId, x.POIID });
                    table.ForeignKey(
                        name: "FK_DetectionPOI_Detection_detectionId",
                        column: x => x.detectionId,
                        principalTable: "Detection",
                        principalColumn: "detectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetectionPOI_POI_POIID",
                        column: x => x.POIID,
                        principalTable: "POI",
                        principalColumn: "POIID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prediction",
                columns: table => new
                {
                    predictionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    detectionId = table.Column<int>(type: "int", nullable: false),
                    predictedFor = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    modelVersion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    predictedFillLevel = table.Column<float>(type: "float", nullable: false),
                    confidence = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prediction", x => x.predictionId);
                    table.ForeignKey(
                        name: "FK_Prediction_Detection_detectionId",
                        column: x => x.detectionId,
                        principalTable: "Detection",
                        principalColumn: "detectionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Detection_trashTypeId",
                table: "Detection",
                column: "trashTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetectionPOI_POIID",
                table: "DetectionPOI",
                column: "POIID");

            migrationBuilder.CreateIndex(
                name: "IX_POI_categoryId",
                table: "POI",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Prediction_detectionId",
                table: "Prediction",
                column: "detectionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_email",
                table: "User",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetectionPOI");

            migrationBuilder.DropTable(
                name: "Prediction");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "POI");

            migrationBuilder.DropTable(
                name: "Detection");

            migrationBuilder.DropTable(
                name: "POICategory");

            migrationBuilder.DropTable(
                name: "TrashType");
        }
    }
}
