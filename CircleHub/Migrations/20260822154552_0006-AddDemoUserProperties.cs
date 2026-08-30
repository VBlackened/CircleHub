using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleHub.Migrations
{
    /// <inheritdoc />
    public partial class _0006AddDemoUserProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DemoLastActivity",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDemo",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DemoLastActivity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDemo",
                table: "AspNetUsers");
        }
    }
}
