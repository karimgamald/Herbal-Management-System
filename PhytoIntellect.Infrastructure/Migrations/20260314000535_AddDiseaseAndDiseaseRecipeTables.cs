using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiseaseAndDiseaseRecipeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiseaseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiseaseType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.DiseaseId);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDiseases",
                columns: table => new
                {
                    RecipeDiseaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    DiseaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDiseases", x => x.RecipeDiseaseId);
                    table.ForeignKey(
                        name: "FK_RecipeDiseases_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Diseases",
                        principalColumn: "DiseaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeDiseases_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Diseases",
                columns: new[] { "DiseaseId", "Description", "DiseaseName", "DiseaseType", "Symptoms" },
                values: new object[,]
                {
                    { 1, "Habitual sleeplessness or inability to sleep.", "Insomnia", "Neurological", "Difficulty falling asleep, waking up often." },
                    { 2, "A common disorder that affects the large intestine.", "Irritable Bowel Syndrome (IBS)", "Gastrointestinal", "Cramping, abdominal pain, bloating, gas." },
                    { 3, "A viral infection of your nose and throat.", "Common Cold", "Respiratory", "Runny nose, sore throat, cough, congestion." },
                    { 4, "A feeling of worry, nervousness, or unease.", "Anxiety", "Psychological", "Restlessness, rapid breathing, increased heart rate." },
                    { 5, "Discomfort in your upper abdomen.", "Indigestion", "Gastrointestinal", "Bloating, nausea, belching." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDiseases_DiseaseId",
                table: "RecipeDiseases",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDiseases_RecipeId",
                table: "RecipeDiseases",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeDiseases");

            migrationBuilder.DropTable(
                name: "Diseases");
        }
    }
}
