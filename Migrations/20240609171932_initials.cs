using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorClient.Migrations
{
    /// <inheritdoc />
    public partial class initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenerationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prompt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NegativePrompt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    GuidanceScale = table.Column<float>(type: "REAL", nullable: false),
                    StyleStrength = table.Column<float>(type: "REAL", nullable: false),
                    InferenceSteps = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenerationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationRequests_GenerationSettings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "GenerationSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenerationRequests_SettingsId",
                table: "GenerationRequests",
                column: "SettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenerationRequests");

            migrationBuilder.DropTable(
                name: "GenerationSettings");
        }
    }
}
