using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Users_TherapistId",
                table: "TherapySessions");

            migrationBuilder.RenameColumn(
                name: "TherapistId",
                table: "TherapySessions",
                newName: "RelationshipId");

            migrationBuilder.RenameIndex(
                name: "IX_TherapySessions_TherapistId",
                table: "TherapySessions",
                newName: "IX_TherapySessions_RelationshipId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TherapySessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiSession",
                table: "TherapySessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "TherapySessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PsychologistId",
                table: "TherapySessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionType",
                table: "TherapySessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Text");

            migrationBuilder.AddColumn<Guid>(
                name: "TherapySessionId",
                table: "EmotionalStates",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TherapistPatientRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PsychologistId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TherapistPatientRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TherapistPatientRelationships_PatientProfiles_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TherapistPatientRelationships_PsychologistProfiles_Psycholo~",
                        column: x => x.PsychologistId,
                        principalTable: "PsychologistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    MessageType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Text"),
                    Attachment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessages_TherapistPatientRelationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "TherapistPatientRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DirectMessages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TherapySessions_PsychologistId",
                table: "TherapySessions",
                column: "PsychologistId");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionalStates_TherapySessionId",
                table: "EmotionalStates",
                column: "TherapySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessage_Sender_Receiver",
                table: "DirectMessages",
                columns: new[] { "SenderId", "ReceiverId" });

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessage_SentAt",
                table: "DirectMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_ReceiverId",
                table: "DirectMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_RelationshipId",
                table: "DirectMessages",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_TherapistPatientRelationship_PsychologistPatientStatus",
                table: "TherapistPatientRelationships",
                columns: new[] { "PsychologistId", "PatientId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TherapistPatientRelationships_PatientId",
                table: "TherapistPatientRelationships",
                column: "PatientId");

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
                name: "FK_TherapySessions_Users_PsychologistId",
                table: "TherapySessions",
                column: "PsychologistId",
                principalTable: "Users",
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
                name: "FK_TherapySessions_TherapistPatientRelationships_RelationshipId",
                table: "TherapySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Users_PsychologistId",
                table: "TherapySessions");

            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "TherapistPatientRelationships");

            migrationBuilder.DropIndex(
                name: "IX_TherapySessions_PsychologistId",
                table: "TherapySessions");

            migrationBuilder.DropIndex(
                name: "IX_EmotionalStates_TherapySessionId",
                table: "EmotionalStates");

            migrationBuilder.DropColumn(
                name: "IsAiSession",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "PsychologistId",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "SessionType",
                table: "TherapySessions");

            migrationBuilder.DropColumn(
                name: "TherapySessionId",
                table: "EmotionalStates");

            migrationBuilder.RenameColumn(
                name: "RelationshipId",
                table: "TherapySessions",
                newName: "TherapistId");

            migrationBuilder.RenameIndex(
                name: "IX_TherapySessions_RelationshipId",
                table: "TherapySessions",
                newName: "IX_TherapySessions_TherapistId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TherapySessions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Scheduled");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_Users_TherapistId",
                table: "TherapySessions",
                column: "TherapistId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
