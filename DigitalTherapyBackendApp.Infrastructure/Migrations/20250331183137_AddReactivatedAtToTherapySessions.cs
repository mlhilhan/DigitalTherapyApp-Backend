using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReactivatedAtToTherapySessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReactivatedAt",
                table: "TherapySessions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReactivatedAt",
                table: "TherapySessions");
        }
    }
}
