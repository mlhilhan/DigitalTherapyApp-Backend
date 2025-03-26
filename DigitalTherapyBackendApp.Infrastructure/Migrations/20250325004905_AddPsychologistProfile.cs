using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPsychologistProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "PsychologistProfiles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "PsychologistProfiles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "PsychologistProfiles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionName",
                table: "PsychologistProfiles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "PsychologistProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "PsychologistProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PsychologistAvailabilitySlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PsychologistProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychologistAvailabilitySlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsychologistAvailabilitySlots_PsychologistProfiles_Psycholo~",
                        column: x => x.PsychologistProfileId,
                        principalTable: "PsychologistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsychologistSpecialties",
                columns: table => new
                {
                    PsychologistsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecialtiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychologistSpecialties", x => new { x.PsychologistsId, x.SpecialtiesId });
                    table.ForeignKey(
                        name: "FK_PsychologistSpecialties_PsychologistProfiles_PsychologistsId",
                        column: x => x.PsychologistsId,
                        principalTable: "PsychologistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PsychologistSpecialties_Specialties_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PsychologistAvailabilitySlots_PsychologistProfileId",
                table: "PsychologistAvailabilitySlots",
                column: "PsychologistProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PsychologistSpecialties_SpecialtiesId",
                table: "PsychologistSpecialties",
                column: "SpecialtiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_Name",
                table: "Specialties",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PsychologistAvailabilitySlots");

            migrationBuilder.DropTable(
                name: "PsychologistSpecialties");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "PsychologistProfiles");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "PsychologistProfiles");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "PsychologistProfiles");

            migrationBuilder.DropColumn(
                name: "InstitutionName",
                table: "PsychologistProfiles");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "PsychologistProfiles");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "PsychologistProfiles");
        }
    }
}
