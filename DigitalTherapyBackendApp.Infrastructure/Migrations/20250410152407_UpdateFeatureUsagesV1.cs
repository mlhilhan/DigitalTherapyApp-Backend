﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeatureUsagesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeatureUsages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeatureUsages");
        }
    }
}
