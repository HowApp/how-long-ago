using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUseless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_liked_records",
                table: "liked_records");

            migrationBuilder.DropPrimaryKey(
                name: "pk_liked_events",
                table: "liked_events");

            migrationBuilder.DropColumn(
                name: "id",
                table: "liked_records");

            migrationBuilder.DropColumn(
                name: "id",
                table: "liked_events");

            migrationBuilder.AddPrimaryKey(
                name: "pk_liked_records",
                table: "liked_records",
                columns: new[] { "record_id", "liked_by_user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_liked_events",
                table: "liked_events",
                columns: new[] { "event_id", "liked_by_user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_liked_records",
                table: "liked_records");

            migrationBuilder.DropPrimaryKey(
                name: "pk_liked_events",
                table: "liked_events");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "liked_records",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "liked_events",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_liked_records",
                table: "liked_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_liked_events",
                table: "liked_events",
                column: "id");
        }
    }
}
