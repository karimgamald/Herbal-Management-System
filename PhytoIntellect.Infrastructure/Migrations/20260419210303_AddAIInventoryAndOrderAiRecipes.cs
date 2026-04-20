using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIInventoryAndOrderAiRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HerbalistAiRecipe",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    AiRecipeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbalistAiRecipe", x => new { x.HerbalistId, x.AiRecipeId });
                    table.ForeignKey(
                        name: "FK_HerbalistAiRecipe_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbalistAiRecipe_Herbalists_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalists",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAiRecipe",
                columns: table => new
                {
                    OrderAiRecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubOrderId = table.Column<int>(type: "int", nullable: false),
                    AiRecipeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAiRecipe", x => x.OrderAiRecipeId);
                    table.ForeignKey(
                        name: "FK_OrderAiRecipe_AiRecipes_AiRecipeId",
                        column: x => x.AiRecipeId,
                        principalTable: "AiRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAiRecipe_SubOrders_SubOrderId",
                        column: x => x.SubOrderId,
                        principalTable: "SubOrders",
                        principalColumn: "SubOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HerbalistAiRecipe_AiRecipeId",
                table: "HerbalistAiRecipe",
                column: "AiRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiRecipe_AiRecipeId",
                table: "OrderAiRecipe",
                column: "AiRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAiRecipe_SubOrderId",
                table: "OrderAiRecipe",
                column: "SubOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HerbalistAiRecipe");

            migrationBuilder.DropTable(
                name: "OrderAiRecipe");
        }
    }
}
