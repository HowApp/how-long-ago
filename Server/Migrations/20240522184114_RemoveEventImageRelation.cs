using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventImageRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_records_storage_images_storage_image_id",
                table: "records");

            migrationBuilder.DropIndex(
                name: "ix_records_storage_image_id",
                table: "records");

            migrationBuilder.DropColumn(
                name: "storage_image_id",
                table: "records");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "storage_image_id",
                table: "records",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_records_storage_image_id",
                table: "records",
                column: "storage_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_records_storage_images_storage_image_id",
                table: "records",
                column: "storage_image_id",
                principalTable: "storage_images",
                principalColumn: "id");
        }
    }
}
