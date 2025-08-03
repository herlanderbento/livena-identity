using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Livena.Identity.infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "revoked_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_revoked_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    purpose = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_codes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_user_codes_user_id",
                table: "user_codes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_revoked_tokens_user_id",
                table: "revoked_tokens",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_revoked_tokens_users",
                table: "revoked_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_codes_users",
                table: "user_codes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_revoked_tokens_users",
                table: "revoked_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_user_codes_users",
                table: "user_codes");

            migrationBuilder.DropIndex(
                name: "i_x_user_codes_user_id",
                table: "user_codes");

            migrationBuilder.DropIndex(
                name: "i_x_revoked_tokens_user_id",
                table: "revoked_tokens");

            migrationBuilder.DropTable(
                name: "user_codes");

            migrationBuilder.DropTable(
                name: "revoked_tokens");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
