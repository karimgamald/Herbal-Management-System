using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedbackConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AiChatRecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "AiChatRecipeId", "PatientId" },
                unique: true,
                filter: "[AiChatRecipeId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks",
                sql: "(CASE WHEN [RecipeId] IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN [AiRecipeId] IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN [AiChatRecipeId] IS NOT NULL THEN 1 ELSE 0 END) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AiChatRecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AiChatRecipeId",
                table: "Feedbacks",
                column: "AiChatRecipeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedback_Target",
                table: "Feedbacks",
                sql: "([RecipeId] IS NOT NULL AND [AiRecipeId] IS NULL) OR ([RecipeId] IS NULL AND [AiRecipeId] IS NOT NULL)");
        }
    }
}
