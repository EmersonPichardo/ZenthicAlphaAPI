using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserTokenType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserId",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "identity",
                table: "UserTokens",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId_Type",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "UserId", "Type" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserTokens_Type_Enum",
                schema: "identity",
                table: "UserTokens",
                sql: "[Type] IN ('EmailConfirmation')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserId_Type",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UserTokens_Type_Enum",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId",
                schema: "identity",
                table: "UserTokens",
                column: "UserId");
        }
    }
}
