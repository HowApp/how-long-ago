using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_record_images_storage_files_main_id",
                table: "record_images");

            migrationBuilder.DropForeignKey(
                name: "fk_record_images_storage_files_thumbnail_id",
                table: "record_images");

            migrationBuilder.DropIndex(
                name: "ix_record_images_main_id",
                table: "record_images");

            migrationBuilder.DropIndex(
                name: "ix_record_images_record_id",
                table: "record_images");

            migrationBuilder.DropIndex(
                name: "ix_record_images_thumbnail_id",
                table: "record_images");

            migrationBuilder.DropColumn(
                name: "image_height",
                table: "record_images");

            migrationBuilder.DropColumn(
                name: "image_width",
                table: "record_images");

            migrationBuilder.DropColumn(
                name: "main_id",
                table: "record_images");

            migrationBuilder.DropColumn(
                name: "thumbnail_height",
                table: "record_images");

            migrationBuilder.DropColumn(
                name: "thumbnail_id",
                table: "record_images");

            migrationBuilder.RenameColumn(
                name: "thumbnail_width",
                table: "record_images",
                newName: "image_id");

            migrationBuilder.CreateIndex(
                name: "ix_record_images_image_id",
                table: "record_images",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "ix_record_images_record_id_image_id",
                table: "record_images",
                columns: new[] { "record_id", "image_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_record_images_storage_images_image_id",
                table: "record_images",
                column: "image_id",
                principalTable: "storage_images",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_record_images_storage_images_image_id",
                table: "record_images");

            migrationBuilder.DropIndex(
                name: "ix_record_images_image_id",
                table: "record_images");

            migrationBuilder.DropIndex(
                name: "ix_record_images_record_id_image_id",
                table: "record_images");

            migrationBuilder.RenameColumn(
                name: "image_id",
                table: "record_images",
                newName: "thumbnail_width");

            migrationBuilder.AddColumn<int>(
                name: "image_height",
                table: "record_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "image_width",
                table: "record_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "main_id",
                table: "record_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "thumbnail_height",
                table: "record_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "thumbnail_id",
                table: "record_images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_record_images_main_id",
                table: "record_images",
                column: "main_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_record_images_record_id",
                table: "record_images",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "ix_record_images_thumbnail_id",
                table: "record_images",
                column: "thumbnail_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_record_images_storage_files_main_id",
                table: "record_images",
                column: "main_id",
                principalTable: "storage_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_record_images_storage_files_thumbnail_id",
                table: "record_images",
                column: "thumbnail_id",
                principalTable: "storage_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
