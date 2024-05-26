using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_record_images_image_id",
                table: "record_images");

            migrationBuilder.CreateIndex(
                name: "ix_record_images_image_id",
                table: "record_images",
                column: "image_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_record_images_image_id",
                table: "record_images");

            migrationBuilder.CreateIndex(
                name: "ix_record_images_image_id",
                table: "record_images",
                column: "image_id");
        }
    }
}
