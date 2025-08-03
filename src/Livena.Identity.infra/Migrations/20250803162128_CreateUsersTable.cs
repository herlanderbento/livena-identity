using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Livena.Identity.infra.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCodes",
                table: "UserCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RevokedTokens",
                table: "RevokedTokens");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "UserCodes",
                newName: "user_codes");

            migrationBuilder.RenameTable(
                name: "RevokedTokens",
                newName: "revoked_tokens");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "users",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "users",
                newName: "birthday");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "users",
                newName: "is_verified");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "user_codes",
                newName: "purpose");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "user_codes",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_codes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_codes",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UsedAt",
                table: "user_codes",
                newName: "used_at");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "user_codes",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_codes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "revoked_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "revoked_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "revoked_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "revoked_tokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "revoked_tokens",
                newName: "access_token");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "purpose",
                table: "user_codes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_codes",
                table: "user_codes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_revoked_tokens",
                table: "revoked_tokens",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "p_k_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_codes",
                table: "user_codes");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_revoked_tokens",
                table: "revoked_tokens");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "user_codes",
                newName: "UserCodes");

            migrationBuilder.RenameTable(
                name: "revoked_tokens",
                newName: "RevokedTokens");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Users",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "birthday",
                table: "Users",
                newName: "Birthday");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_verified",
                table: "Users",
                newName: "IsVerified");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "purpose",
                table: "UserCodes",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "UserCodes",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserCodes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserCodes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "used_at",
                table: "UserCodes",
                newName: "UsedAt");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "UserCodes",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UserCodes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RevokedTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "RevokedTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "RevokedTokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "RevokedTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "access_token",
                table: "RevokedTokens",
                newName: "AccessToken");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Purpose",
                table: "UserCodes",
                type: "integer",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCodes",
                table: "UserCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RevokedTokens",
                table: "RevokedTokens",
                column: "Id");
        }
    }
}
