using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DigitalTherapyBackendApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyTips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTipCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTipCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyTipCategoryTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTipCategoryTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTipCategoryTranslations_DailyTipCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DailyTipCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyTips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    TipKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsBookmarked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTips_DailyTipCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DailyTipCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DailyTipTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipId = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTipTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTipTranslations_DailyTips_TipId",
                        column: x => x.TipId,
                        principalTable: "DailyTips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTipCategories_CategoryKey",
                table: "DailyTipCategories",
                column: "CategoryKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyTipCategoryTranslations_CategoryId_LanguageCode",
                table: "DailyTipCategoryTranslations",
                columns: new[] { "CategoryId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyTips_CategoryId",
                table: "DailyTips",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTips_TipKey",
                table: "DailyTips",
                column: "TipKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyTipTranslations_TipId_LanguageCode",
                table: "DailyTipTranslations",
                columns: new[] { "TipId", "LanguageCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTipCategoryTranslations");

            migrationBuilder.DropTable(
                name: "DailyTipTranslations");

            migrationBuilder.DropTable(
                name: "DailyTips");

            migrationBuilder.DropTable(
                name: "DailyTipCategories");
        }
    }
}
