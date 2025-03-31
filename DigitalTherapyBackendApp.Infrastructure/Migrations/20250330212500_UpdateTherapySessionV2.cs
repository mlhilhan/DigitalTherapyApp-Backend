using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTherapySessionV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientProfiles_Users_UserId1",
                table: "PatientProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions");

            migrationBuilder.DropIndex(
                name: "IX_PatientProfiles_UserId",
                table: "PatientProfiles");

            migrationBuilder.DropIndex(
                name: "IX_PatientProfiles_UserId1",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PatientProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientProfileId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PatientProfiles_UserId",
                table: "PatientProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PatientProfileId",
                table: "Users",
                column: "PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PatientProfiles_PatientProfileId",
                table: "Users",
                column: "PatientProfileId",
                principalTable: "PatientProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_PatientProfiles_PatientProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PatientProfileId",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PatientProfiles_UserId",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "PatientProfileId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "PatientProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_UserId",
                table: "PatientProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_UserId1",
                table: "PatientProfiles",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProfiles_Users_UserId1",
                table: "PatientProfiles",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
