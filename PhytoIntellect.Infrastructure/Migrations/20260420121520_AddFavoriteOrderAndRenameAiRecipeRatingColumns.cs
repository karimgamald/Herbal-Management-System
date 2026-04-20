using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteOrderAndRenameAiRecipeRatingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientAverageRating",
                table: "AiRecipes");

            migrationBuilder.DropColumn(
                name: "PatientTotalRatings",
                table: "AiRecipes");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "AiRecipes",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AiRecipes");

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
        }
    }
}
