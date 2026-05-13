using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiChatRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "AiRecipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AiChatRecipe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    UserPrompt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RecommendedRecipeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MainHerb = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraindications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchPercentage = table.Column<double>(type: "float", nullable: false),
                    OtherPossibilities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiChatRecipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiChatRecipe_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiChatRecipe_PatientId",
                table: "AiChatRecipe",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiChatRecipe");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "AiRecipes");
        }
    }
}
