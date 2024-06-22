using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDeletionFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_IsDeleted",
                table: "UsersRoles",
                column: "IsDeleted",
                filter: "IsDeleted = 'FALSE'");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted",
                filter: "IsDeleted = 'FALSE'");

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermissions_IsDeleted",
                table: "RolesPermissions",
                column: "IsDeleted",
                filter: "IsDeleted = 'FALSE'");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles",
                column: "IsDeleted",
                filter: "IsDeleted = 'FALSE'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersRoles_IsDeleted",
                table: "UsersRoles");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RolesPermissions_IsDeleted",
                table: "RolesPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles");
        }
    }
}
