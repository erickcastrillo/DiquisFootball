using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diquis.Infrastructure.Persistence.Migrations.BaseDb
{
    /// <inheritdoc />
    public partial class AddTenantProvisioningStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastProvisioningAttempt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningError",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastProvisioningAttempt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ProvisioningError",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tenants");
        }
    }
}
