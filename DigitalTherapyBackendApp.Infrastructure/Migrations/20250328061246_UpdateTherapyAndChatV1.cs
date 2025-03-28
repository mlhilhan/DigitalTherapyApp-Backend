using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTherapyAndChatV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmotionalStates_TherapySessions_TherapySessionId",
                table: "EmotionalStates");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_TherapistPatientRelationships_RelationshipId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Users_PatientId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Users_PsychologistId",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "SessionType",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "TherapySessions");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TherapySessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Scheduled");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLink",
                table: "TherapySessions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiSession",
                table: "TherapySessions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TherapySessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TherapySessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiGenerated",
                table: "SessionMessages",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "SessionMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionalStates_TherapySessions_TherapySessionId",
                table: "EmotionalStates",
                column: "TherapySessionId",
                principalTable: "TherapySessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_PsychologistProfiles_PsychologistId",
                table: "TherapySessions",
                column: "PsychologistId",
                principalTable: "PsychologistProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_TherapistPatientRelationships_RelationshipId",
                table: "TherapySessions",
                column: "RelationshipId",
                principalTable: "TherapistPatientRelationships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmotionalStates_TherapySessions_TherapySessionId",
                table: "EmotionalStates");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_PatientProfiles_PatientId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_PsychologistProfiles_PsychologistId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_TherapistPatientRelationships_RelationshipId",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TherapySessions");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TherapySessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLink",
                table: "TherapySessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiSession",
                table: "TherapySessions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "SessionType",
                table: "TherapySessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Text");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "TherapySessions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiGenerated",
                table: "SessionMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "SessionMessages",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_EmotionalStates_TherapySessions_TherapySessionId",
                table: "EmotionalStates",
                column: "TherapySessionId",
                principalTable: "TherapySessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_TherapistPatientRelationships_RelationshipId",
                table: "TherapySessions",
                column: "RelationshipId",
                principalTable: "TherapistPatientRelationships",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_Users_PatientId",
                table: "TherapySessions",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_Users_PsychologistId",
                table: "TherapySessions",
                column: "PsychologistId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
