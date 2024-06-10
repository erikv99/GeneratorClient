using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneratorClient.Migrations
{
    /// <inheritdoc />
    public partial class AddedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "GenerationResponses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "GenerationResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "GenerationResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_GenerationResponses_RequestId",
                table: "GenerationResponses",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationResponses_GenerationRequests_RequestId",
                table: "GenerationResponses",
                column: "RequestId",
                principalTable: "GenerationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenerationResponses_GenerationRequests_RequestId",
                table: "GenerationResponses");

            migrationBuilder.DropIndex(
                name: "IX_GenerationResponses_RequestId",
                table: "GenerationResponses");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "GenerationResponses");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "GenerationResponses");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "GenerationResponses");
        }
    }
}
