using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ComponentsCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RolesPermissions_Component_Enum",
                table: "RolesPermissions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolesPermissions_Component_Enum",
                table: "RolesPermissions",
                sql: "[Component] IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RolesPermissions_Component_Enum",
                table: "RolesPermissions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolesPermissions_Component_Enum",
                table: "RolesPermissions",
                sql: "[Component] IN (0)");
        }
    }
}
