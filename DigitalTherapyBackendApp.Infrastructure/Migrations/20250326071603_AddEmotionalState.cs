using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmotionalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmotionalStates_Users_UserId",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "Mood",
                table: "EmotionalStates");

            migrationBuilder.RenameColumn(
                name: "MoodIntensity",
                table: "EmotionalStates",
                newName: "MoodLevel");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "EmotionalStates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmotionalStates",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "EmotionalStates",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Factors",
                table: "EmotionalStates",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBookmarked",
                table: "EmotionalStates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmotionalStates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EmotionalStates",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmotionalStates_Date",
                table: "EmotionalStates",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionalStates_IsBookmarked",
                table: "EmotionalStates",
                column: "IsBookmarked");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionalStates_IsDeleted",
                table: "EmotionalStates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionalStates_UserId_Date",
                table: "EmotionalStates",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmotionalStates_Date",
                table: "EmotionalStates");

            migrationBuilder.DropIndex(
                name: "IX_EmotionalStates_IsBookmarked",
                table: "EmotionalStates");

            migrationBuilder.DropIndex(
                name: "IX_EmotionalStates_IsDeleted",
                table: "EmotionalStates");

            migrationBuilder.DropIndex(
                name: "IX_EmotionalStates_UserId_Date",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "Factors",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "IsBookmarked",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EmotionalStates");

            migrationBuilder.RenameColumn(
                name: "MoodLevel",
                table: "EmotionalStates",
                newName: "MoodIntensity");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "EmotionalStates",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmotionalStates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "EmotionalStates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionalStates_Users_UserId",
                table: "EmotionalStates",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
