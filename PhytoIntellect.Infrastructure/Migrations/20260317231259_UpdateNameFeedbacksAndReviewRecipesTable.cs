using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameFeedbacksAndReviewRecipesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Patients_PatientId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Recipes_RecipeId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipe_Herbalists_HerbalistId",
                table: "ReviewRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipe_Recipes_RecipeId",
                table: "ReviewRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewRecipe",
                table: "ReviewRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback");

            migrationBuilder.RenameTable(
                name: "ReviewRecipe",
                newName: "ReviewRecipes");

            migrationBuilder.RenameTable(
                name: "Feedback",
                newName: "Feedbacks");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipe_RecipeId_HerbalistId",
                table: "ReviewRecipes",
                newName: "IX_ReviewRecipes_RecipeId_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipe_HerbalistId",
                table: "ReviewRecipes",
                newName: "IX_ReviewRecipes_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_RecipeId_PatientId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_RecipeId_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_PatientId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewRecipes",
                table: "ReviewRecipes",
                column: "ReviewRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Patients_PatientId",
                table: "Feedbacks",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Recipes_RecipeId",
                table: "Feedbacks",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_Herbalists_HerbalistId",
                table: "ReviewRecipes",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_Recipes_RecipeId",
                table: "ReviewRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Patients_PatientId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Recipes_RecipeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_Herbalists_HerbalistId",
                table: "ReviewRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_Recipes_RecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewRecipes",
                table: "ReviewRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "ReviewRecipes",
                newName: "ReviewRecipe");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "Feedback");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipes_RecipeId_HerbalistId",
                table: "ReviewRecipe",
                newName: "IX_ReviewRecipe_RecipeId_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewRecipes_HerbalistId",
                table: "ReviewRecipe",
                newName: "IX_ReviewRecipe_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedback",
                newName: "IX_Feedback_RecipeId_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_PatientId",
                table: "Feedback",
                newName: "IX_Feedback_PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewRecipe",
                table: "ReviewRecipe",
                column: "ReviewRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Patients_PatientId",
                table: "Feedback",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Recipes_RecipeId",
                table: "Feedback",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipe_Herbalists_HerbalistId",
                table: "ReviewRecipe",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipe_Recipes_RecipeId",
                table: "ReviewRecipe",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
