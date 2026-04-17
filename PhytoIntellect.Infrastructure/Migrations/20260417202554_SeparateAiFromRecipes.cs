using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeparateAiFromRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_Recipes_RecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "CreatedByAI",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "HerbalistAverageRating",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "HerbalistTotalRatings",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "AllProbabilitiesJson",
                table: "AiRecipes");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "ReviewRecipes",
                newName: "AiRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipes_RecipeId_HerbalistId",
                table: "ReviewRecipes",
                newName: "IX_ReviewRecipes_AiRecipeId_HerbalistId");

            migrationBuilder.AddColumn<int>(
                name: "AiRecipeId",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HerbalistAverageRating",
                table: "AiRecipes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "HerbalistTotalRatings",
                table: "AiRecipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "PatientAverageRating",
                table: "AiRecipes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PatientTotalRatings",
                table: "AiRecipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AiRecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "AiRecipeId", "PatientId" },
                unique: true,
                filter: "[AiRecipeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "RecipeId", "PatientId" },
                unique: true,
                filter: "[RecipeId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks",
                sql: "([RecipeId] IS NOT NULL AND [AiRecipeId] IS NULL) OR ([RecipeId] IS NULL AND [AiRecipeId] IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AiRecipes_AiRecipeId",
                table: "Feedbacks",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_AiRecipes_AiRecipeId",
                table: "ReviewRecipes",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AiRecipes_AiRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_AiRecipes_AiRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AiRecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AiRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "HerbalistAverageRating",
                table: "AiRecipes");

            migrationBuilder.DropColumn(
                name: "HerbalistTotalRatings",
                table: "AiRecipes");

            migrationBuilder.DropColumn(
                name: "PatientAverageRating",
                table: "AiRecipes");

            migrationBuilder.DropColumn(
                name: "PatientTotalRatings",
                table: "AiRecipes");

            migrationBuilder.RenameColumn(
                name: "AiRecipeId",
                table: "ReviewRecipes",
                newName: "RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes",
                newName: "IX_ReviewRecipes_RecipeId_HerbalistId");

            migrationBuilder.AddColumn<bool>(
                name: "CreatedByAI",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<float>(
                name: "HerbalistAverageRating",
                table: "Recipes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "HerbalistTotalRatings",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AllProbabilitiesJson",
                table: "AiRecipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "RecipeId", "PatientId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_Recipes_RecipeId",
                table: "ReviewRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
