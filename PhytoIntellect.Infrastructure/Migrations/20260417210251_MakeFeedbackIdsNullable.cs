using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    public partial class MakeFeedbackIdsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Feedbacks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "RecipeId", "PatientId" },
                unique: true,
                filter: "[RecipeId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RecipeId_PatientId",
                table: "Feedbacks",
                columns: new[] { "RecipeId", "PatientId" },
                unique: true);
        }
    }
}