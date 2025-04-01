using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedForTherapy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TherapySessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TherapySessions");
        }
    }
}
