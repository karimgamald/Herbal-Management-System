using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewRecipeAndHerbalistRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "ReviewRecipe",
                columns: table => new
                {
                    ReviewRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    HerbalistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRecipe", x => x.ReviewRecipeId);
                    table.CheckConstraint("CK_ReviewRecipe_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5");
                    table.ForeignKey(
                        name: "FK_ReviewRecipe_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewRecipe_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipe_HerbalistId",
                table: "ReviewRecipe",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipe_RecipeId_HerbalistId",
                table: "ReviewRecipe",
                columns: new[] { "RecipeId", "HerbalistId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewRecipe");

            migrationBuilder.DropColumn(
                name: "HerbalistAverageRating",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "HerbalistTotalRatings",
                table: "Recipes");
        }
    }
}
