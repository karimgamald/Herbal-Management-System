using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToRecipeHerb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Herb_Herbalist_AddedByHerbalistId",
                table: "Herb");

            migrationBuilder.DropForeignKey(
                name: "FK_Herbalist_Users_UserId",
                table: "Herbalist");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistHerb_Herb_HerbId",
                table: "HerbalistHerb");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistHerb_Herbalist_HerbalistId",
                table: "HerbalistHerb");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Herbalist_HerbalistId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHerb_Herb_HerbId",
                table: "RecipeHerb");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHerb_Recipe_RecipeId",
                table: "RecipeHerb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeHerb",
                table: "RecipeHerb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipe",
                table: "Recipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistHerb",
                table: "HerbalistHerb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herbalist",
                table: "Herbalist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herb",
                table: "Herb");

            migrationBuilder.RenameTable(
                name: "RecipeHerb",
                newName: "RecipeHerbs");

            migrationBuilder.RenameTable(
                name: "Recipe",
                newName: "Recipes");

            migrationBuilder.RenameTable(
                name: "MedicalHistory",
                newName: "MedicalHistories");

            migrationBuilder.RenameTable(
                name: "HerbalistHerb",
                newName: "HerbalistHerbs");

            migrationBuilder.RenameTable(
                name: "Herbalist",
                newName: "Herbalists");

            migrationBuilder.RenameTable(
                name: "Herb",
                newName: "Herbs");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeHerb_RecipeId",
                table: "RecipeHerbs",
                newName: "IX_RecipeHerbs_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeHerb_HerbId",
                table: "RecipeHerbs",
                newName: "IX_RecipeHerbs_HerbId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipe_HerbalistId",
                table: "Recipes",
                newName: "IX_Recipes_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistory_PatientId",
                table: "MedicalHistories",
                newName: "IX_MedicalHistories_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistHerb_HerbId",
                table: "HerbalistHerbs",
                newName: "IX_HerbalistHerbs_HerbId");

            migrationBuilder.RenameIndex(
                name: "IX_Herbalist_UserId",
                table: "Herbalists",
                newName: "IX_Herbalists_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Herb_AddedByHerbalistId",
                table: "Herbs",
                newName: "IX_Herbs_AddedByHerbalistId");

            migrationBuilder.AddColumn<float>(
                name: "Quantity",
                table: "RecipeHerbs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeHerbs",
                table: "RecipeHerbs",
                column: "RecipeHerbId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories",
                column: "MedicalHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistHerbs",
                table: "HerbalistHerbs",
                columns: new[] { "HerbalistId", "HerbId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herbalists",
                table: "Herbalists",
                column: "HerbalistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herbs",
                table: "Herbs",
                column: "HerbId");

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistHerbs_Herbalists_HerbalistId",
                table: "HerbalistHerbs",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistHerbs_Herbs_HerbId",
                table: "HerbalistHerbs",
                column: "HerbId",
                principalTable: "Herbs",
                principalColumn: "HerbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Herbalists_Users_UserId",
                table: "Herbalists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Herbs_Herbalists_AddedByHerbalistId",
                table: "Herbs",
                column: "AddedByHerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHerbs_Herbs_HerbId",
                table: "RecipeHerbs",
                column: "HerbId",
                principalTable: "Herbs",
                principalColumn: "HerbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHerbs_Recipes_RecipeId",
                table: "RecipeHerbs",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Herbalists_HerbalistId",
                table: "Recipes",
                column: "HerbalistId",
                principalTable: "Herbalists",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistHerbs_Herbalists_HerbalistId",
                table: "HerbalistHerbs");

            migrationBuilder.DropForeignKey(
                name: "FK_HerbalistHerbs_Herbs_HerbId",
                table: "HerbalistHerbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Herbalists_Users_UserId",
                table: "Herbalists");

            migrationBuilder.DropForeignKey(
                name: "FK_Herbs_Herbalists_AddedByHerbalistId",
                table: "Herbs");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHerbs_Herbs_HerbId",
                table: "RecipeHerbs");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHerbs_Recipes_RecipeId",
                table: "RecipeHerbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Herbalists_HerbalistId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeHerbs",
                table: "RecipeHerbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herbs",
                table: "Herbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herbalists",
                table: "Herbalists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HerbalistHerbs",
                table: "HerbalistHerbs");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RecipeHerbs");

            migrationBuilder.RenameTable(
                name: "Recipes",
                newName: "Recipe");

            migrationBuilder.RenameTable(
                name: "RecipeHerbs",
                newName: "RecipeHerb");

            migrationBuilder.RenameTable(
                name: "MedicalHistories",
                newName: "MedicalHistory");

            migrationBuilder.RenameTable(
                name: "Herbs",
                newName: "Herb");

            migrationBuilder.RenameTable(
                name: "Herbalists",
                newName: "Herbalist");

            migrationBuilder.RenameTable(
                name: "HerbalistHerbs",
                newName: "HerbalistHerb");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_HerbalistId",
                table: "Recipe",
                newName: "IX_Recipe_HerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeHerbs_RecipeId",
                table: "RecipeHerb",
                newName: "IX_RecipeHerb_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeHerbs_HerbId",
                table: "RecipeHerb",
                newName: "IX_RecipeHerb_HerbId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "MedicalHistory",
                newName: "IX_MedicalHistory_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Herbs_AddedByHerbalistId",
                table: "Herb",
                newName: "IX_Herb_AddedByHerbalistId");

            migrationBuilder.RenameIndex(
                name: "IX_Herbalists_UserId",
                table: "Herbalist",
                newName: "IX_Herbalist_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HerbalistHerbs_HerbId",
                table: "HerbalistHerb",
                newName: "IX_HerbalistHerb_HerbId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipe",
                table: "Recipe",
                column: "RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeHerb",
                table: "RecipeHerb",
                column: "RecipeHerbId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory",
                column: "MedicalHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herb",
                table: "Herb",
                column: "HerbId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herbalist",
                table: "Herbalist",
                column: "HerbalistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HerbalistHerb",
                table: "HerbalistHerb",
                columns: new[] { "HerbalistId", "HerbId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Herb_Herbalist_AddedByHerbalistId",
                table: "Herb",
                column: "AddedByHerbalistId",
                principalTable: "Herbalist",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Herbalist_Users_UserId",
                table: "Herbalist",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistHerb_Herb_HerbId",
                table: "HerbalistHerb",
                column: "HerbId",
                principalTable: "Herb",
                principalColumn: "HerbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbalistHerb_Herbalist_HerbalistId",
                table: "HerbalistHerb",
                column: "HerbalistId",
                principalTable: "Herbalist",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Herbalist_HerbalistId",
                table: "Recipe",
                column: "HerbalistId",
                principalTable: "Herbalist",
                principalColumn: "HerbalistId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHerb_Herb_HerbId",
                table: "RecipeHerb",
                column: "HerbId",
                principalTable: "Herb",
                principalColumn: "HerbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHerb_Recipe_RecipeId",
                table: "RecipeHerb",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
