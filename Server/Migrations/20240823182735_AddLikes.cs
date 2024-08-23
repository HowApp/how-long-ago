using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "liked_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    liked_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_liked_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_liked_events_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_liked_events_users_liked_by_user_id",
                        column: x => x.liked_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "liked_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    liked_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_liked_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_liked_records_records_record_id",
                        column: x => x.record_id,
                        principalTable: "records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_liked_records_users_liked_by_user_id",
                        column: x => x.liked_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_liked_events_event_id_liked_by_user_id",
                table: "liked_events",
                columns: new[] { "event_id", "liked_by_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_liked_events_liked_by_user_id",
                table: "liked_events",
                column: "liked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_records_liked_by_user_id",
                table: "liked_records",
                column: "liked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_records_record_id_liked_by_user_id",
                table: "liked_records",
                columns: new[] { "record_id", "liked_by_user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "liked_events");

            migrationBuilder.DropTable(
                name: "liked_records");
        }
    }
}
