using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiChatEcosystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AiChatRecipeId",
                table: "ReviewRecipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiChatRecipeId",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HerbalistAverageRating",
                table: "AiChatRecipe",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "HerbalistTotalRatings",
                table: "AiChatRecipe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "AiChatRecipe",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HerbalistAiChatRecipe",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    AiChatRecipeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbalistAiChatRecipe", x => new { x.HerbalistId, x.AiChatRecipeId });
                    table.ForeignKey(
                        name: "FK_HerbalistAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                        column: x => x.AiChatRecipeId,
                        principalTable: "AiChatRecipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbalistAiChatRecipe_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderAiChatRecipe",
                columns: table => new
                {
                    OrderAiChatRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubOrderId = table.Column<int>(type: "int", nullable: false),
                    AiChatRecipeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAiChatRecipe", x => x.OrderAiChatRecipeId);
                    table.ForeignKey(
                        name: "FK_OrderAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                        column: x => x.AiChatRecipeId,
                        principalTable: "AiChatRecipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAiChatRecipe_SubOrders_SubOrderId",
                        column: x => x.SubOrderId,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipes_AiChatRecipeId",
                table: "ReviewRecipes",
                column: "AiChatRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AiChatRecipeId",
                table: "Feedbacks",
                column: "AiChatRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_HerbalistAiChatRecipe_AiChatRecipeId",
                table: "HerbalistAiChatRecipe",
                column: "AiChatRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiChatRecipe_AiChatRecipeId",
                table: "OrderAiChatRecipe",
                column: "AiChatRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiChatRecipe_SubOrderId",
                table: "OrderAiChatRecipe",
                column: "SubOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AiChatRecipe_AiChatRecipeId",
                table: "Feedbacks",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipe_AiChatRecipeId",
                table: "ReviewRecipes",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AiChatRecipe_AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipe_AiChatRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropTable(
                name: "HerbalistAiChatRecipe");

            migrationBuilder.DropTable(
                name: "OrderAiChatRecipe");

            migrationBuilder.DropIndex(
                name: "IX_ReviewRecipes_AiChatRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AiChatRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropColumn(
                name: "AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "HerbalistAverageRating",
                table: "AiChatRecipe");

            migrationBuilder.DropColumn(
                name: "HerbalistTotalRatings",
                table: "AiChatRecipe");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AiChatRecipe");
        }
    }
}
