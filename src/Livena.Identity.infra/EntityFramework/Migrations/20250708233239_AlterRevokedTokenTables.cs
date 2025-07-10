using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Livena.Identity.infra.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AlterRevokedTokenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "RevokedTokens");

            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "RevokedTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "RevokedTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TokenType",
                table: "RevokedTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
