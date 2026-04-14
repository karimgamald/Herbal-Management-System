using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiRecipeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    HeightCm = table.Column<double>(type: "float", nullable: false),
                    Bmi = table.Column<double>(type: "float", nullable: false),
                    SeverityScore = table.Column<int>(type: "int", nullable: false),
                    SystolicBp = table.Column<int>(type: "int", nullable: false),
                    DiastolicBp = table.Column<int>(type: "int", nullable: false),
                    TemperatureCelsius = table.Column<double>(type: "float", nullable: false),
                    HeartRateBpm = table.Column<int>(type: "int", nullable: false),
                    SymptomDurationDays = table.Column<int>(type: "int", nullable: false),
                    HasDiabetes = table.Column<bool>(type: "bit", nullable: false),
                    HasHypertension = table.Column<bool>(type: "bit", nullable: false),
                    HasAllergies = table.Column<bool>(type: "bit", nullable: false),
                    IsPregnant = table.Column<bool>(type: "bit", nullable: false),
                    IsSmoker = table.Column<bool>(type: "bit", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedRecipeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    PreparationInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CautionWarning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllProbabilitiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiRecipes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiRecipes_PatientId",
                table: "AiRecipes",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiRecipes");
        }
    }
}
