using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedNamedAiModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiRecipe_AiRecipes_AiRecipeId",
                table: "HerbalistAiRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiRecipe_Herbalists_HerbalistId",
                table: "HerbalistAiRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiRecipe_AiRecipes_AiRecipeId",
                table: "OrderAiRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiRecipe_SubOrders_SubOrderId",
                table: "OrderAiRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderAiRecipe",
                table: "OrderAiRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistAiRecipe",
                table: "HerbalistAiRecipe");

            migrationBuilder.RenameTable(
                name: "OrderAiRecipe",
                newName: "OrderAiRecipes");

            migrationBuilder.RenameTable(
                name: "HerbalistAiRecipe",
                newName: "HerbalistAiRecipes");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiRecipe_SubOrderId",
                table: "OrderAiRecipes",
                newName: "IX_OrderAiRecipes_SubOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiRecipe_AiRecipeId",
                table: "OrderAiRecipes",
                newName: "IX_OrderAiRecipes_AiRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistAiRecipe_AiRecipeId",
                table: "HerbalistAiRecipes",
                newName: "IX_HerbalistAiRecipes_AiRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderAiRecipes",
                table: "OrderAiRecipes",
                column: "OrderAiRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistAiRecipes",
                table: "HerbalistAiRecipes",
                columns: new[] { "HerbalistId", "AiRecipeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiRecipes_AiRecipes_AiRecipeId",
                table: "HerbalistAiRecipes",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiRecipes_Herbalists_HerbalistId",
                table: "HerbalistAiRecipes",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiRecipes_AiRecipes_AiRecipeId",
                table: "OrderAiRecipes",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiRecipes_SubOrders_SubOrderId",
                table: "OrderAiRecipes",
                column: "SubOrderId",
                principalTable: "SubOrders",
                principalColumn: "SubOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiRecipes_AiRecipes_AiRecipeId",
                table: "HerbalistAiRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiRecipes_Herbalists_HerbalistId",
                table: "HerbalistAiRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiRecipes_AiRecipes_AiRecipeId",
                table: "OrderAiRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiRecipes_SubOrders_SubOrderId",
                table: "OrderAiRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderAiRecipes",
                table: "OrderAiRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistAiRecipes",
                table: "HerbalistAiRecipes");

            migrationBuilder.RenameTable(
                name: "OrderAiRecipes",
                newName: "OrderAiRecipe");

            migrationBuilder.RenameTable(
                name: "HerbalistAiRecipes",
                newName: "HerbalistAiRecipe");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiRecipes_SubOrderId",
                table: "OrderAiRecipe",
                newName: "IX_OrderAiRecipe_SubOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiRecipes_AiRecipeId",
                table: "OrderAiRecipe",
                newName: "IX_OrderAiRecipe_AiRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistAiRecipes_AiRecipeId",
                table: "HerbalistAiRecipe",
                newName: "IX_HerbalistAiRecipe_AiRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderAiRecipe",
                table: "OrderAiRecipe",
                column: "OrderAiRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistAiRecipe",
                table: "HerbalistAiRecipe",
                columns: new[] { "HerbalistId", "AiRecipeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiRecipe_AiRecipes_AiRecipeId",
                table: "HerbalistAiRecipe",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiRecipe_Herbalists_HerbalistId",
                table: "HerbalistAiRecipe",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiRecipe_AiRecipes_AiRecipeId",
                table: "OrderAiRecipe",
                column: "AiRecipeId",
                principalTable: "AiRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiRecipe_SubOrders_SubOrderId",
                table: "OrderAiRecipe",
                column: "SubOrderId",
                principalTable: "SubOrders",
                principalColumn: "SubOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
