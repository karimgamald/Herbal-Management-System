using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableNamedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiChatRecipe_Patients_PatientId",
                table: "AiChatRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AiChatRecipe_AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                table: "HerbalistAiChatRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiChatRecipe_Herbalists_HerbalistId",
                table: "HerbalistAiChatRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                table: "OrderAiChatRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiChatRecipe_SubOrders_SubOrderId",
                table: "OrderAiChatRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipe_AiChatRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderAiChatRecipe",
                table: "OrderAiChatRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistAiChatRecipe",
                table: "HerbalistAiChatRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AiChatRecipe",
                table: "AiChatRecipe");

            migrationBuilder.RenameTable(
                name: "OrderAiChatRecipe",
                newName: "OrderAiChatRecipes");

            migrationBuilder.RenameTable(
                name: "HerbalistAiChatRecipe",
                newName: "HerbalistAiChatRecipes");

            migrationBuilder.RenameTable(
                name: "AiChatRecipe",
                newName: "AiChatRecipes");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiChatRecipe_SubOrderId",
                table: "OrderAiChatRecipes",
                newName: "IX_OrderAiChatRecipes_SubOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiChatRecipe_AiChatRecipeId",
                table: "OrderAiChatRecipes",
                newName: "IX_OrderAiChatRecipes_AiChatRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistAiChatRecipe_AiChatRecipeId",
                table: "HerbalistAiChatRecipes",
                newName: "IX_HerbalistAiChatRecipes_AiChatRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_AiChatRecipe_PatientId",
                table: "AiChatRecipes",
                newName: "IX_AiChatRecipes_PatientId");

            migrationBuilder.AlterColumn<int>(
                name: "AiRecipeId",
                table: "ReviewRecipes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderAiChatRecipes",
                table: "OrderAiChatRecipes",
                column: "OrderAiChatRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistAiChatRecipes",
                table: "HerbalistAiChatRecipes",
                columns: new[] { "HerbalistId", "AiChatRecipeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiChatRecipes",
                table: "AiChatRecipes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes",
                columns: new[] { "AiRecipeId", "HerbalistId" },
                unique: true,
                filter: "[AiRecipeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AiChatRecipes_Patients_PatientId",
                table: "AiChatRecipes",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AiChatRecipes_AiChatRecipeId",
                table: "Feedbacks",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiChatRecipes_AiChatRecipes_AiChatRecipeId",
                table: "HerbalistAiChatRecipes",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiChatRecipes_Herbalists_HerbalistId",
                table: "HerbalistAiChatRecipes",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiChatRecipes_AiChatRecipes_AiChatRecipeId",
                table: "OrderAiChatRecipes",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiChatRecipes_SubOrders_SubOrderId",
                table: "OrderAiChatRecipes",
                column: "SubOrderId",
                principalTable: "SubOrders",
                principalColumn: "SubOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipes_AiChatRecipeId",
                table: "ReviewRecipes",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiChatRecipes_Patients_PatientId",
                table: "AiChatRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AiChatRecipes_AiChatRecipeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiChatRecipes_AiChatRecipes_AiChatRecipeId",
                table: "HerbalistAiChatRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistAiChatRecipes_Herbalists_HerbalistId",
                table: "HerbalistAiChatRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiChatRecipes_AiChatRecipes_AiChatRecipeId",
                table: "OrderAiChatRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAiChatRecipes_SubOrders_SubOrderId",
                table: "OrderAiChatRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipes_AiChatRecipeId",
                table: "ReviewRecipes");

            migrationBuilder.DropIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderAiChatRecipes",
                table: "OrderAiChatRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistAiChatRecipes",
                table: "HerbalistAiChatRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AiChatRecipes",
                table: "AiChatRecipes");

            migrationBuilder.RenameTable(
                name: "OrderAiChatRecipes",
                newName: "OrderAiChatRecipe");

            migrationBuilder.RenameTable(
                name: "HerbalistAiChatRecipes",
                newName: "HerbalistAiChatRecipe");

            migrationBuilder.RenameTable(
                name: "AiChatRecipes",
                newName: "AiChatRecipe");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiChatRecipes_SubOrderId",
                table: "OrderAiChatRecipe",
                newName: "IX_OrderAiChatRecipe_SubOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAiChatRecipes_AiChatRecipeId",
                table: "OrderAiChatRecipe",
                newName: "IX_OrderAiChatRecipe_AiChatRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistAiChatRecipes_AiChatRecipeId",
                table: "HerbalistAiChatRecipe",
                newName: "IX_HerbalistAiChatRecipe_AiChatRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_AiChatRecipes_PatientId",
                table: "AiChatRecipe",
                newName: "IX_AiChatRecipe_PatientId");

            migrationBuilder.AlterColumn<int>(
                name: "AiRecipeId",
                table: "ReviewRecipes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderAiChatRecipe",
                table: "OrderAiChatRecipe",
                column: "OrderAiChatRecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistAiChatRecipe",
                table: "HerbalistAiChatRecipe",
                columns: new[] { "HerbalistId", "AiChatRecipeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiChatRecipe",
                table: "AiChatRecipe",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipes_AiRecipeId_HerbalistId",
                table: "ReviewRecipes",
                columns: new[] { "AiRecipeId", "HerbalistId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AiChatRecipe_Patients_PatientId",
                table: "AiChatRecipe",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AiChatRecipe_AiChatRecipeId",
                table: "Feedbacks",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                table: "HerbalistAiChatRecipe",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistAiChatRecipe_Herbalists_HerbalistId",
                table: "HerbalistAiChatRecipe",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiChatRecipe_AiChatRecipe_AiChatRecipeId",
                table: "OrderAiChatRecipe",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAiChatRecipe_SubOrders_SubOrderId",
                table: "OrderAiChatRecipe",
                column: "SubOrderId",
                principalTable: "SubOrders",
                principalColumn: "SubOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipes_AiChatRecipe_AiChatRecipeId",
                table: "ReviewRecipes",
                column: "AiChatRecipeId",
                principalTable: "AiChatRecipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
