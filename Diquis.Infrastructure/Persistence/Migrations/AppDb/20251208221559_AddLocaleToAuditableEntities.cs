using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diquis.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddLocaleToAuditableEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locale",
                table: "Products");
        }
    }
}
