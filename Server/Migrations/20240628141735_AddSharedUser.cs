using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace How.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "access",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "shared_users",
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
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shared_users_users_user_shared_id",
                        column: x => x.user_shared_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shared_users_user_owner_id_user_shared_id",
                table: "shared_users",
                columns: new[] { "user_owner_id", "user_shared_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shared_users_user_shared_id",
                table: "shared_users",
                column: "user_shared_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shared_users");

            migrationBuilder.DropColumn(
                name: "access",
                table: "events");
        }
    }
}
