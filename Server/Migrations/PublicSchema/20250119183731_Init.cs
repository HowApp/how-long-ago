using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace How.Server.Migrations.PublicSchema
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "storage_files",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hash = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    extension = table.Column<string>(type: "text", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "storage_images",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    image_height = table.Column<int>(type: "integer", nullable: false),
                    image_width = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_height = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_width = table.Column<int>(type: "integer", nullable: false),
                    main_id = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_storage_images_storage_files_main_id",
                        column: x => x.main_id,
                        principalSchema: "public",
                        principalTable: "storage_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_storage_images_storage_files_thumbnail_id",
                        column: x => x.thumbnail_id,
                        principalSchema: "public",
                        principalTable: "storage_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    last_name = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_suspended = table.Column<bool>(type: "boolean", nullable: false),
                    storage_image_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_users_storage_images_storage_image_id",
                        column: x => x.storage_image_id,
                        principalSchema: "public",
                        principalTable: "storage_images",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    access = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    owner_id = table.Column<int>(type: "integer", nullable: false),
                    storage_image_id = table.Column<int>(type: "integer", nullable: true),
                    created_by_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    changed_by_id = table.Column<int>(type: "integer", nullable: true),
                    changed_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_events_storage_images_storage_image_id",
                        column: x => x.storage_image_id,
                        principalSchema: "public",
                        principalTable: "storage_images",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_events_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shared_users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_owner_id = table.Column<int>(type: "integer", nullable: false),
                    user_shared_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shared_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_shared_users_users_user_owner_id",
                        column: x => x.user_owner_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shared_users_users_user_shared_id",
                        column: x => x.user_shared_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "liked_events",
                schema: "public",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    liked_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_liked_events", x => new { x.event_id, x.liked_by_user_id });
                    table.ForeignKey(
                        name: "fk_liked_events_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "public",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_liked_events_users_liked_by_user_id",
                        column: x => x.liked_by_user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "records",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    created_by_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_records_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "public",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "saved_events",
                schema: "public",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_events", x => new { x.event_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_saved_events_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "public",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_saved_events_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "liked_records",
                schema: "public",
                columns: table => new
                {
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    liked_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_liked_records", x => new { x.record_id, x.liked_by_user_id });
                    table.ForeignKey(
                        name: "fk_liked_records_records_record_id",
                        column: x => x.record_id,
                        principalSchema: "public",
                        principalTable: "records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_liked_records_users_liked_by_user_id",
                        column: x => x.liked_by_user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "record_images",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    image_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_record_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_record_images_records_record_id",
                        column: x => x.record_id,
                        principalSchema: "public",
                        principalTable: "records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_record_images_storage_images_image_id",
                        column: x => x.image_id,
                        principalSchema: "public",
                        principalTable: "storage_images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_events_owner_id",
                schema: "public",
                table: "events",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_storage_image_id",
                schema: "public",
                table: "events",
                column: "storage_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_events_event_id_liked_by_user_id",
                schema: "public",
                table: "liked_events",
                columns: new[] { "event_id", "liked_by_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_liked_events_liked_by_user_id",
                schema: "public",
                table: "liked_events",
                column: "liked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_records_liked_by_user_id",
                schema: "public",
                table: "liked_records",
                column: "liked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_records_record_id_liked_by_user_id",
                schema: "public",
                table: "liked_records",
                columns: new[] { "record_id", "liked_by_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_record_images_image_id",
                schema: "public",
                table: "record_images",
                column: "image_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_record_images_record_id_image_id",
                schema: "public",
                table: "record_images",
                columns: new[] { "record_id", "image_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_records_event_id",
                schema: "public",
                table: "records",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_events_event_id_user_id",
                schema: "public",
                table: "saved_events",
                columns: new[] { "event_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_saved_events_user_id",
                schema: "public",
                table: "saved_events",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_shared_users_user_owner_id_user_shared_id",
                schema: "public",
                table: "shared_users",
                columns: new[] { "user_owner_id", "user_shared_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shared_users_user_shared_id",
                schema: "public",
                table: "shared_users",
                column: "user_shared_id");

            migrationBuilder.CreateIndex(
                name: "ix_storage_files_hash",
                schema: "public",
                table: "storage_files",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_storage_files_path",
                schema: "public",
                table: "storage_files",
                column: "path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_storage_images_main_id",
                schema: "public",
                table: "storage_images",
                column: "main_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_storage_images_thumbnail_id",
                schema: "public",
                table: "storage_images",
                column: "thumbnail_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_storage_image_id",
                schema: "public",
                table: "users",
                column: "storage_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_user_id",
                schema: "public",
                table: "users",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "liked_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "liked_records",
                schema: "public");

            migrationBuilder.DropTable(
                name: "record_images",
                schema: "public");

            migrationBuilder.DropTable(
                name: "saved_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "shared_users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "records",
                schema: "public");

            migrationBuilder.DropTable(
                name: "events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "storage_images",
                schema: "public");

            migrationBuilder.DropTable(
                name: "storage_files",
                schema: "public");
        }
    }
}
